using AutoMapper;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Services.Basket.Domain.DTOs.CartDtos;
using ETicaretAPI.Services.Basket.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Application.Mappings.AutoMapper.Profiles;

public class CartProfile : Profile
{
    public CartProfile()
    {
        CreateMap<Cart, GetCartDto>()
            .ForMember(dest => dest.CouponCode,
                opt => opt.MapFrom(src => src.Coupon != null ? src.Coupon.Code : null))
            .ForMember(dest => dest.Items,
                opt => opt.MapFrom(src => src.Items));

        CreateMap<CartItem, GetCartItemDto>();

        CreateMap<AddCartItemDto, CartItem>();

        CreateMap<PagedResult<Cart>, PagedResult<GetCartDto>>();
    }
}
