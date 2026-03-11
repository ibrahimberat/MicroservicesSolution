using MediatR;
using ProductService.Application.DTOs;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Queries
{
    public class GetProductByIdQuery : IRequest<ProductDto?>
    {
        public Guid Id { get; set; }
    }

    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICacheService _cacheService;

        public GetProductByIdQueryHandler(
            IProductRepository productRepository,
            ICacheService cacheService)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
        }

        public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"product:{request.Id}";
            var cachedProduct = await _cacheService.GetAsync<ProductDto>(cacheKey);
            
            if (cachedProduct != null)
            {
                return cachedProduct;
            }

            var product = await _productRepository.GetByIdAsync(request.Id);
            if (product == null)
            {
                return null;
            }

            var productDto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                Category = product.Category,
                SKU = product.SKU,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };

            await _cacheService.SetAsync(cacheKey, productDto, TimeSpan.FromMinutes(10));

            return productDto;
        }
    }
}
