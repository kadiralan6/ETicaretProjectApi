using AutoMapper;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Services.Basket.Domain.DTOs.CartDtos;
using ETicaretAPI.Services.Basket.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Application.Mappings.AutoMapper.Profiles;

public class CartItemsProfile : Profile
{
    public CartItemsProfile()
    {
        CreateMap<CartItems, GetCartItemDto>();
        CreateMap<AddCartItemDto, CartItems>();
        CreateMap<PagedResult<CartItems>, PagedResult<GetCartItemDto>>();
    }
}
