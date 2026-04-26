using ETicaretAPI.Common.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Domain.Entities;

public class Cart : Entity<int>
{
    public int UserId { get; set; }
    public int? CouponId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Total { get; set; }

    // Navigation properties
    public Coupon? Coupon { get; set; }
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
