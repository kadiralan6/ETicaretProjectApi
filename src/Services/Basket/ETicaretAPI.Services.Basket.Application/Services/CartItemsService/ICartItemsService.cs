using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Services.Basket.Application.DTOs;
using ETicaretAPI.Services.Basket.Domain.DTOs.CartDtos;

namespace ETicaretAPI.Services.Basket.Application.Services.CartItemsService;

public interface ICartItemsService
{
    Task<ApiResponse<GetBasketDto>> GetBasketByUserIdAsync(CancellationToken cancellationToken = default);

    Task<ApiResponse<PagedResult<GetCartItemDto>>> GetCartItemsFilterAsync(GetCartForAdminFilterDto filterDto, CancellationToken cancellationToken = default);

    Task<ApiResponse<GetCartItemDto>> AddItemAsync(AddCartItemDto dto, CancellationToken cancellationToken = default);

    Task<ApiResponse<GetCartItemDto>> UpdateItemAsync(UpdateCartItemDto dto, CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> RemoveItemAsync(int cartItemId, CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> ApplyCouponAsync(ApplyCouponDto dto, CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> RemoveCouponAsync(int cartItemId, CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> ClearCartAsync(CancellationToken cancellationToken = default);

    Task<ApiResponse<GetCartItemCountDto>> GetItemCountAsync(CancellationToken cancellationToken = default);
}
