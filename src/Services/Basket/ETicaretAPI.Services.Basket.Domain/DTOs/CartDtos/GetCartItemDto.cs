using ETicaretAPI.Common.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Domain.DTOs.CartDtos;

public class GetCartItemDto : BaseDto
{
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSlug { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;
}
