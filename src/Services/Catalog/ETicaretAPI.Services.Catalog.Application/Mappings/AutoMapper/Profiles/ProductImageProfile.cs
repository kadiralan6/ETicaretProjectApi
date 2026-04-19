using AutoMapper;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Common.Application.DTOs.CatalogDtos;
using ETicaretAPI.Services.Catalog.Domain.DTOs.ProductImageDtos;
using ETicaretAPI.Services.Catalog.Domain.Entities;

namespace ETicaretAPI.Services.Catalog.Application.Mappings.AutoMapper.Profiles;

public class ProductImageProfile : Profile
{
    public ProductImageProfile()
    {
        CreateMap<ProductImage, CreateProductImageDto>().ReverseMap();
        CreateMap<ProductImage, UpdateProductImageDto>().ReverseMap();
        CreateMap<ProductImage, GetProductImageDto>().ReverseMap();
        CreateMap<PagedResult<ProductImage>, PagedResult<GetProductImageDto>>().ReverseMap();
    }
}
