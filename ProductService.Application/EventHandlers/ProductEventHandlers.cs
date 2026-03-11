using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Domain.Events;

namespace ProductService.Application.EventHandlers
{
    public class ProductCreatedEventHandler : INotificationHandler<ProductCreatedEvent>
    {
        private readonly ILogger<ProductCreatedEventHandler> _logger;

        public ProductCreatedEventHandler(ILogger<ProductCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Product created: {ProductId} - {ProductName} at {Price:C}",
                notification.ProductId, notification.Name, notification.Price);
            return Task.CompletedTask;
        }
    }

    public class ProductUpdatedEventHandler : INotificationHandler<ProductUpdatedEvent>
    {
        private readonly ILogger<ProductUpdatedEventHandler> _logger;

        public ProductUpdatedEventHandler(ILogger<ProductUpdatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(ProductUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Product updated: {ProductId} - {ProductName} at {Price:C}",
                notification.ProductId, notification.Name, notification.Price);
            return Task.CompletedTask;
        }
    }

    public class ProductDeletedEventHandler : INotificationHandler<ProductDeletedEvent>
    {
        private readonly ILogger<ProductDeletedEventHandler> _logger;

        public ProductDeletedEventHandler(ILogger<ProductDeletedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(ProductDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Product deleted: {ProductId}", notification.ProductId);
            return Task.CompletedTask;
        }
    }
}