using MediatR;
using ProductService.Application.DTOs;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Queries
{
    public class GetProductsByCategoryQuery : IRequest<IEnumerable<ProductDto>>
    {
        public string Category { get; set; } = string.Empty;
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }

    public class GetProductsByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQuery, IEnumerable<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICacheService _cacheService;

        public GetProductsByCategoryQueryHandler(
            IProductRepository productRepository,
            ICacheService cacheService)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<ProductDto>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"products:category:{request.Category}:{request.Page ?? 1}:{request.PageSize ?? 10}";

            var cachedProducts = await _cacheService.GetAsync<IEnumerable<ProductDto>>(cacheKey);
            if (cachedProducts != null)
            {
                return cachedProducts;
            }

            var products = await _productRepository.GetByCategoryAsync(request.Category);

            var productDtos = products.Select(p => new ProductDto
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
            });

            if (request.Page.HasValue && request.PageSize.HasValue)
            {
                productDtos = productDtos
                    .Skip((request.Page.Value - 1) * request.PageSize.Value)
                    .Take(request.PageSize.Value);
            }

            var result = productDtos.ToList();
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }
    }
}