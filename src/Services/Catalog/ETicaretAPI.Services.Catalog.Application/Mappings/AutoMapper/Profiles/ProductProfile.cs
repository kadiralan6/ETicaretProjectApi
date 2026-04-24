using AutoMapper;
using ETicaretAPI.Common.Application.Results.Concrete;
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
            .ForMember(d => d.ParentCategoryId, o => o.MapFrom(s => s.Category != null ? s.Category.ParentCategoryId : null))
            .ForMember(d => d.ParentCategoryName, o => o.MapFrom(s => s.Category != null && s.Category.ParentCategory != null ? s.Category.ParentCategory.Name : null))
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category != null ? s.Category.Name : null))
            .ForMember(d => d.BrandName, o => o.MapFrom(s => s.Brand != null ? s.Brand.Name : null));
        CreateMap<PagedResult<Product>, PagedResult<GetProductDto>>().ReverseMap();

    }
}