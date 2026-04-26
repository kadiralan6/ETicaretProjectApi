namespace ETicaretAPI.Services.Basket.Domain.DTOs.CartDtos;

public class AddCartItemDto
{
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string ProductSlug { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
}
