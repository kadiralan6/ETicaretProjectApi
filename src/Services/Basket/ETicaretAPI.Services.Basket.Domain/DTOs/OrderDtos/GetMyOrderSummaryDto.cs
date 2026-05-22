using ETicaretAPI.Services.Basket.Domain.Enums;

namespace ETicaretAPI.Services.Basket.Domain.DTOs.OrderDtos;

public class GetMyOrderSummaryDto
{
    public int OrderId { get; set; }
    public string? OrderNumber { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public OrderStatusEnum Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
