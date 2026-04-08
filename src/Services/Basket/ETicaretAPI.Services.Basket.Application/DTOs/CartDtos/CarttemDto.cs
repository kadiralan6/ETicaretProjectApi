namespace ETicaretAPI.Services.Basket.Application.DTOs.CartDtos;

public class CartItemDto
{
  public int ProductId { get; set; }
  public string ProductName { get; set; } = string.Empty;
  public string? ProductImageUrl { get; set; }
  public int Quantity { get; set; }
  public decimal UnitPrice { get; set; }
}