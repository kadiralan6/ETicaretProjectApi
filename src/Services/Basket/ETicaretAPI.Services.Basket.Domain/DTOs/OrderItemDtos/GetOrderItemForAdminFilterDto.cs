using ETicaretAPI.Services.Basket.Domain.Enums;

namespace ETicaretAPI.Services.Basket.Domain.DTOs.OrderItemDtos;

public class GetOrderItemForAdminFilterDto : BaseFilterDto<OrderItemOrderByEnum>
{
    public int? UserId { get; set; }
    public int? ProductId { get; set; }
    public int? CouponId { get; set; }
    public string? OrderNumber { get; set; }
}