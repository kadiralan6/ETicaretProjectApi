using ETicaretAPI.Common.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Domain.Entities;

public class OrderItem : Entity<int>
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public decimal Price { get; set; }
    public decimal? Discount { get; set; }
    public decimal? TotalPrice { get; set; } //kdv dahil toplam fiyat
    public decimal TotalNetPrice { get; set; } //kdv dahil indirimli fiyat
    public decimal VatAmount { get; set; }
    public int Quantity { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int? CouponId { get; set; }
}