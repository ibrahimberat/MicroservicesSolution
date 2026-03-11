using MediatR;
using FluentValidation;
using AutoMapper;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Events;
using Microsoft.Extensions.Logging;

namespace ProductService.Application.Commands
{
    public class CreateProductCommand : IRequest<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? Category { get; set; }
        public string? SKU { get; set; }
    }

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<CreateProductCommandHandler> _logger;

        public CreateProductCommandHandler(
            IProductRepository productRepository,
            IMapper mapper,
            IMediator mediator,
            ILogger<CreateProductCommandHandler> logger)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // Check if SKU already exists
            if (!string.IsNullOrEmpty(request.SKU) && await _productRepository.SkuExistsAsync(request.SKU))
            {
                throw new InvalidOperationException($"Product with SKU {request.SKU} already exists");
            }

            var product = _mapper.Map<Product>(request);
            product.Id = Guid.NewGuid();
            product.CreatedAt = DateTime.UtcNow;
            product.CreatedBy = "System"; // In real app, get from current user

            var createdProduct = await _productRepository.AddAsync(product);

            _logger.LogInformation("Product created: {ProductId} - {ProductName}", createdProduct.Id, createdProduct.Name);

            // Publish event for other microservices
            await _mediator.Publish(new ProductCreatedEvent
            {
                ProductId = createdProduct.Id,
                Name = createdProduct.Name,
                Price = createdProduct.Price,
                CreatedAt = createdProduct.CreatedAt,
                CreatedBy = createdProduct.CreatedBy
            }, cancellationToken);

            return createdProduct.Id;
        }
    }

    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
            RuleFor(x => x.SKU).MaximumLength(50).When(x => !string.IsNullOrEmpty(x.SKU));
        }
    }
}