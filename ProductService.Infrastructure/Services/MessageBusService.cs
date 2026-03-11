using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using ProductService.Application.Interfaces;

namespace ProductService.Infrastructure.Services
{
    public class MessageBusService : IProductEventPublisher, System.IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<MessageBusService> _logger;
        private readonly string _exchangeName;

        public MessageBusService(IConfiguration configuration, ILogger<MessageBusService> logger)
        {
            _logger = logger;
            _exchangeName = configuration["RabbitMQ:ExchangeName"] ?? "product.events";
            
            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
                UserName = configuration["RabbitMQ:UserName"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest",
                Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
                VirtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            
            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic, durable: true);
        }

        public async Task PublishProductCreatedEvent(System.Guid productId, string name, decimal price)
        {
            var eventData = new
            {
                EventType = "ProductCreated",
                ProductId = productId,
                Name = name,
                Price = price,
                Timestamp = System.DateTime.UtcNow
            };

            await PublishEvent("product.created", eventData);
        }

        public async Task PublishProductUpdatedEvent(System.Guid productId, string name, decimal price)
        {
            var eventData = new
            {
                EventType = "ProductUpdated",
                ProductId = productId,
                Name = name,
                Price = price,
                Timestamp = System.DateTime.UtcNow
            };

            await PublishEvent("product.updated", eventData);
        }

        public async Task PublishProductDeletedEvent(System.Guid productId)
        {
            var eventData = new
            {
                EventType = "ProductDeleted",
                ProductId = productId,
                Timestamp = System.DateTime.UtcNow
            };

            await PublishEvent("product.deleted", eventData);
        }

        private Task PublishEvent(string routingKey, object eventData)
        {
            try
            {
                var json = JsonSerializer.Serialize(eventData);
                var body = Encoding.UTF8.GetBytes(json);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.ContentType = "application/json";
                properties.Timestamp = new AmqpTimestamp(System.DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                _channel.BasicPublish(
                    exchange: _exchangeName,
                    routingKey: routingKey,
                    basicProperties: properties,
                    body: body);

                _logger.LogInformation("Event published: {RoutingKey} - {EventData}", routingKey, json);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error publishing event to RabbitMQ: {RoutingKey}", routingKey);
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
