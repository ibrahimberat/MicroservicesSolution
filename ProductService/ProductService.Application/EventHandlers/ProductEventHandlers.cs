using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Domain.Events;
using ProductService.Application.Interfaces;

namespace ProductService.Application.EventHandlers
{
    public class ProductCreatedEventHandler : INotificationHandler<ProductCreatedEvent>
    {
        private readonly ILogger<ProductCreatedEventHandler> _logger;
        private readonly IProductEventPublisher _eventPublisher;

        public ProductCreatedEventHandler(
            ILogger<ProductCreatedEventHandler> logger,
            IProductEventPublisher eventPublisher)
        {
            _logger = logger;
            _eventPublisher = eventPublisher;
        }

        public async Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProductCreatedEvent received: {ProductId} - {ProductName}", 
                notification.ProductId, notification.Name);
            
            // RabbitMQ'ya event gönder
            await _eventPublisher.PublishProductCreatedEvent(
                notification.ProductId, 
                notification.Name, 
                notification.Price);
        }
    }

    public class ProductUpdatedEventHandler : INotificationHandler<ProductUpdatedEvent>
    {
        private readonly ILogger<ProductUpdatedEventHandler> _logger;
        private readonly IProductEventPublisher _eventPublisher;

        public ProductUpdatedEventHandler(
            ILogger<ProductUpdatedEventHandler> logger,
            IProductEventPublisher eventPublisher)
        {
            _logger = logger;
            _eventPublisher = eventPublisher;
        }

        public async Task Handle(ProductUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProductUpdatedEvent received: {ProductId} - {ProductName}", 
                notification.ProductId, notification.Name);
            
            await _eventPublisher.PublishProductUpdatedEvent(
                notification.ProductId, 
                notification.Name, 
                notification.Price);
        }
    }

    public class ProductDeletedEventHandler : INotificationHandler<ProductDeletedEvent>
    {
        private readonly ILogger<ProductDeletedEventHandler> _logger;
        private readonly IProductEventPublisher _eventPublisher;

        public ProductDeletedEventHandler(
            ILogger<ProductDeletedEventHandler> logger,
            IProductEventPublisher eventPublisher)
        {
            _logger = logger;
            _eventPublisher = eventPublisher;
        }

        public async Task Handle(ProductDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProductDeletedEvent received: {ProductId}", notification.ProductId);
            
            await _eventPublisher.PublishProductDeletedEvent(notification.ProductId);
        }
    }
}
