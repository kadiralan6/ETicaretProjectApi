using AutoMapper;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Services.Basket.Domain.DTOs.CouponDtos;
using ETicaretAPI.Services.Basket.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Application.Mappings.AutoMapper.Profiles;

public class CouponProfile : Profile
{
    public CouponProfile()
    {
        CreateMap<Coupon, GetCouponDto>();
        CreateMap<CreateCouponDto, Coupon>();
        CreateMap<UpdateCouponDto, Coupon>();
        CreateMap<PagedResult<Coupon>, PagedResult<GetCouponDto>>();
    }
}
