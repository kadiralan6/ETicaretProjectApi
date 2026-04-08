using ETicaretAPI.Common.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Domain.Entities;

public class Cart : Entity<int>
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? OrderNumber { get; set; }
    public int CouponId { get; set; }
}


