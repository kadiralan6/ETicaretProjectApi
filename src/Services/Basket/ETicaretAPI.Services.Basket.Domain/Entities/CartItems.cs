using ETicaretAPI.Common.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Domain.Entities;

public class CartItems : Entity<int>
{
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public int Quantity { get; set; }
    public int? CouponId { get; set; }
    public string? OrderNumber { get; set; }

    public Coupon? Coupon { get; set; }
}
