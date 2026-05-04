using ETicaretAPI.Common.Application.DTOs.CatalogDtos;

namespace ETicaretAPI.Services.Basket.Application.DTOs;

public class GetBasketItemDto
{
    public int CartItemId { get; set; }
    public int UserId { get; set; }
    public int Quantity { get; set; }
    public int? CouponId { get; set; }
    public string? OrderNumber { get; set; }

    // Product details from Catalog service
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductSlug { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public string? CategoryName { get; set; }
    public string? BrandName { get; set; }
    public List<GetProductImageDto> Images { get; set; } = [];
}
