using ETicaretAPI.Services.Basket.Domain.Enums;

namespace ETicaretAPI.Services.Basket.Domain.DTOs.OrderDtos;

public class GetOrderForAdminFilterDto : BaseFilterDto<OrderOrderByEnum>
{
    public int? UserId { get; set; }
    public int? ProductId { get; set; }
    public int? CouponId { get; set; }
    public string? OrderNumber { get; set; }
}