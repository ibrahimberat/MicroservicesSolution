using MediatR;
using ProductService.Application.DTOs;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Queries
{
    public class ProductsByCategoryQuery : IRequest<IEnumerable<ProductDto>>
    {
        public string Category { get; set; } = string.Empty;
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }

    public class ProductsByCategoryQueryHandler : IRequestHandler<ProductsByCategoryQuery, IEnumerable<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICacheService _cacheService;

        public ProductsByCategoryQueryHandler(IProductRepository productRepository, ICacheService cacheService)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<ProductDto>> Handle(ProductsByCategoryQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"products:category:{request.Category}:{request.Page ?? 1}:{request.PageSize ?? 10}";
            
            var cachedProducts = await _cacheService.GetAsync<IEnumerable<ProductDto>>(cacheKey);
            if (cachedProducts != null) return cachedProducts;

            var products = await _productRepository.GetByCategoryAsync(request.Category);
            
            var result = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                Category = p.Category,
                SKU = p.SKU,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList();

            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }
    }
}
