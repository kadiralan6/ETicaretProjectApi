using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Services.Basket.Application.DTOs;
using ETicaretAPI.Services.Basket.Domain.DTOs.CartDtos;

namespace ETicaretAPI.Services.Basket.Application.Services.CartItemsService;

public interface ICartItemsService
{
    Task<ApiResponse<List<GetBasketItemDto>>> GetBasketByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task<ApiResponse<PagedResult<GetCartItemDto>>> GetCartItemsFilterAsync(GetCartForAdminFilterDto filterDto, CancellationToken cancellationToken = default);

    Task<ApiResponse<GetCartItemDto>> AddItemAsync(AddCartItemDto dto, CancellationToken cancellationToken = default);

    Task<ApiResponse<GetCartItemDto>> UpdateItemAsync(int userId, UpdateCartItemDto dto, CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> RemoveItemAsync(int userId, int cartItemId, CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> ApplyCouponAsync(int userId, ApplyCouponDto dto, CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> RemoveCouponAsync(int userId, CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> ClearCartAsync(int userId, CancellationToken cancellationToken = default);

    Task<ApiResponse<GetCartItemCountDto>> GetItemCountAsync(int userId, CancellationToken cancellationToken = default);
}
