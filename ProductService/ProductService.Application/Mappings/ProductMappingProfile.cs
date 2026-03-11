using AutoMapper;
using ProductService.Domain.Entities;
using ProductService.Application.Commands;
using ProductService.Application.DTOs;

namespace ProductService.Application.Mappings
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<CreateProductCommand, Product>();
            CreateMap<UpdateProductCommand, Product>();
            CreateMap<Product, ProductDto>();
        }
    }
}
