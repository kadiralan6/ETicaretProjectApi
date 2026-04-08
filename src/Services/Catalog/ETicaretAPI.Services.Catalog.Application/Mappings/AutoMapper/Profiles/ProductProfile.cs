using AutoMapper;
using ETicaretAPI.Services.Catalog.Domain.DTOs.ProductDtos;
using ETicaretAPI.Services.Catalog.Domain.Entities;

namespace ETicaretAPI.Services.Catalog.Application.Mappings.AutoMapper.Profiles;

public class ProductProfile : Profile
{
  public ProductProfile()
  {
    CreateMap<Product, CreateProductDto>().ReverseMap();
    CreateMap<Product, UpdateProductDto>().ReverseMap();
    CreateMap<Product, GetProductDto>()
        .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category != null ? s.Category.Name : null))
        .ForMember(d => d.BrandName, o => o.MapFrom(s => s.Brand != null ? s.Brand.Name : null));
  }
}
