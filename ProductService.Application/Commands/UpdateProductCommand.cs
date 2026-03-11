using MediatR;
using FluentValidation;
using AutoMapper;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Events;
using Microsoft.Extensions.Logging;

namespace ProductService.Application.Commands
{
    public class UpdateProductCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? Category { get; set; }
    }

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<UpdateProductCommandHandler> _logger;

        public UpdateProductCommandHandler(
            IProductRepository productRepository,
            IMapper mapper,
            IMediator mediator,
            ILogger<UpdateProductCommandHandler> logger)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var existingProduct = await _productRepository.GetByIdAsync(request.Id);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Product with ID {request.Id} not found");
            }

            _mapper.Map(request, existingProduct);
            existingProduct.UpdatedAt = DateTime.UtcNow;
            existingProduct.UpdatedBy = "System"; // In real app, get from current user

            await _productRepository.UpdateAsync(existingProduct);

            _logger.LogInformation("Product updated: {ProductId}", existingProduct.Id);

            // Publish event for other microservices
            await _mediator.Publish(new ProductUpdatedEvent
            {
                ProductId = existingProduct.Id,
                Name = existingProduct.Name,
                Price = existingProduct.Price,
                UpdatedAt = existingProduct.UpdatedAt.Value,
                UpdatedBy = existingProduct.UpdatedBy
            }, cancellationToken);

            return Unit.Value;
        }
    }

    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
        }
    }
}