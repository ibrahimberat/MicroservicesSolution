using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using ProductService.Application.Interfaces;

namespace ProductService.Infrastructure.Services
{
    public class MessageBusService : IProductEventPublisher, IDisposable
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
                VirtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/",
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            
            // Exchange oluştur
            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic, durable: true, autoDelete: false);
            
            // Queue oluştur ve bind et
            var queueName = _channel.QueueDeclare(
                queue: "product.events.queue", 
                durable: true, 
                exclusive: false, 
                autoDelete: false,
                arguments: null
            );
            
            _channel.QueueBind(
                queue: "product.events.queue", 
                exchange: _exchangeName, 
                routingKey: "product.*"
            );
            
            _logger.LogInformation("RabbitMQ connection established. Exchange: {Exchange}, Queue: product.events.queue", _exchangeName);
        }

        public async Task PublishProductCreatedEvent(Guid productId, string name, decimal price)
        {
            var eventData = new
            {
                EventType = "ProductCreated",
                ProductId = productId,
                Name = name,
                Price = price,
                Timestamp = DateTime.UtcNow
            };

            await PublishEvent("product.created", eventData);
        }

        public async Task PublishProductUpdatedEvent(Guid productId, string name, decimal price)
        {
            var eventData = new
            {
                EventType = "ProductUpdated",
                ProductId = productId,
                Name = name,
                Price = price,
                Timestamp = DateTime.UtcNow
            };

            await PublishEvent("product.updated", eventData);
        }

        public async Task PublishProductDeletedEvent(Guid productId)
        {
            var eventData = new
            {
                EventType = "ProductDeleted",
                ProductId = productId,
                Timestamp = DateTime.UtcNow
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
                properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                _channel.BasicPublish(
                    exchange: _exchangeName,
                    routingKey: routingKey,
                    basicProperties: properties,
                    body: body);

                _logger.LogInformation("Event published: {RoutingKey} - {EventData}", routingKey, json);
            }
            catch (Exception ex)
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
