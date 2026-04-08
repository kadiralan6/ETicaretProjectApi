using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Services.Basket.Application.DTOs.CartDtos;

namespace ETicaretAPI.Services.Basket.Application.Services.CartService;

public interface ICartService
{
  Task<ApiResponse<CartDto>> GetCartAsync(CancellationToken cancellationToken = default);
}
