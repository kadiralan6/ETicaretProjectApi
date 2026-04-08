namespace ETicaretAPI.Services.Basket.Application.DTOs.CartDtos;

public class CartDto
{
  public int UserId { get; set; }
  public List<CartItemDto> Items { get; set; } = new();
  public decimal TotalPrice { get; set; }
  public string? CouponCode { get; set; }
  public decimal DiscountAmount { get; set; }
  public decimal FinalPrice { get; set; }
}

