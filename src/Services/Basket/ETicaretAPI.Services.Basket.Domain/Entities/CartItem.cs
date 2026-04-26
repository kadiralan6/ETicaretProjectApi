using ETicaretAPI.Common.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Domain.Entities;

public class CartItem : Entity<int>
{
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSlug { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }

    // Navigation properties
    public Cart Cart { get; set; } = null!;
}
