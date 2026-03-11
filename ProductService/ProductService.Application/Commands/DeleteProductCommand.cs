using MediatR;
using FluentValidation;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Events;
using Microsoft.Extensions.Logging;

namespace ProductService.Application.Commands
{
    public class DeleteProductCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMediator _mediator;
        private readonly ICacheService _cacheService;
        private readonly ILogger<DeleteProductCommandHandler> _logger;

        public DeleteProductCommandHandler(
            IProductRepository productRepository,
            IMediator mediator,
            ICacheService cacheService,
            ILogger<DeleteProductCommandHandler> logger)
        {
            _productRepository = productRepository;
            _mediator = mediator;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {request.Id} not found");
            }

            await _productRepository.DeleteAsync(request.Id);
            
            await _cacheService.RemoveAsync($"product:{request.Id}");
            await _cacheService.RemoveByPatternAsync("products:*");
            
            _logger.LogInformation("Product deleted: {ProductId}", request.Id);

            await _mediator.Publish(new ProductDeletedEvent
            {
                ProductId = request.Id,
                DeletedAt = DateTime.UtcNow
            }, cancellationToken);

            return true;
        }
    }

    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
