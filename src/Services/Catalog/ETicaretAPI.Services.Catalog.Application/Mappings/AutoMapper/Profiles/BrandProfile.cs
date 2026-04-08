using AutoMapper;
using ETicaretAPI.Services.Catalog.Domain.DTOs.BrandDtos;
using ETicaretAPI.Services.Catalog.Domain.Entities;

namespace ETicaretAPI.Services.Catalog.Application.Mappings.AutoMapper.Profiles;

public class BrandProfile : Profile
{
  public BrandProfile()
  {
    CreateMap<Brand, CreateBrandDto>().ReverseMap();
    CreateMap<Brand, UpdateBrandDto>().ReverseMap();
    CreateMap<Brand, GetBrandDto>().ReverseMap();
  }
}
