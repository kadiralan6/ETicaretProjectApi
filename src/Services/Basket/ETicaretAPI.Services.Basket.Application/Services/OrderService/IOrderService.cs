using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Services.Basket.Domain.DTOs.OrderDtos;

namespace ETicaretAPI.Services.Basket.Application.Services.OrderService;

public interface IOrderService
{
    Task<ApiResponse<PagedResult<GetOrderDto>>> GetOrdersFilterAsync(GetOrderForAdminFilterDto filterDto, CancellationToken cancellationToken = default);
    Task<ApiResponse<GetOrderDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<GetOrderDto>> CreateAsync(CreateOrderDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<GetOrderDto>> UpdateAsync(UpdateOrderDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}