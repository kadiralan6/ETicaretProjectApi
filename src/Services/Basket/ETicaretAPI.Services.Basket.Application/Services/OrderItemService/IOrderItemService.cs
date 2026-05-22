using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Services.Basket.Domain.DTOs.OrderItemDtos;

namespace ETicaretAPI.Services.Basket.Application.Services.OrderItemService;

public interface IOrderItemService
{
    Task<ApiResponse<PagedResult<GetOrderItemDto>>> GetOrderItemsFilterAsync(GetOrderItemForAdminFilterDto filterDto, CancellationToken cancellationToken = default);
    Task<ApiResponse<GetOrderItemDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<GetOrderItemDto>> CreateAsync(CreateOrderItemDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<GetOrderItemDto>> UpdateAsync(UpdateOrderItemDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}