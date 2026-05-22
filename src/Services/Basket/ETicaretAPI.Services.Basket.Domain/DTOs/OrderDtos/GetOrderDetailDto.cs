using ETicaretAPI.Services.Basket.Domain.Enums;

namespace ETicaretAPI.Services.Basket.Domain.DTOs.OrderDtos;

public class GetOrderDetailDto
{
    public int OrderId { get; set; }
    public string? OrderNumber { get; set; }
    public OrderStatusEnum Status { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<GetOrderDetailItemDto> Items { get; set; } = [];
    public OrderAddressDto? Address { get; set; }
    public OrderBuyerDto? Buyer { get; set; }
}
