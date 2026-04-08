using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Services.Basket.Application.DTOs.CartDtos;
using ETicaretAPI.Services.Basket.Application.Services.CartService;

public class CartManager : ICartService
{
    public Task<ApiResponse<CartDto>> GetCartAsync( CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}