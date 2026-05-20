namespace ETicaretAPI.Services.Basket.Application.DTOs;

public class GetBasketDto
{
    public List<GetBasketItemDto> Items { get; set; } = [];

    public int TotalQuantity { get; set; }
    public int UniqueItemCount { get; set; }

    public decimal SubTotal { get; set; }
    public decimal ShippingCost { get; set; }

    public AppliedCouponDto? AppliedCoupon { get; set; }
    public AppliedCampaignDto? AppliedCampaign { get; set; }

    public decimal TotalDiscount { get; set; }
    public decimal Total { get; set; }
}
