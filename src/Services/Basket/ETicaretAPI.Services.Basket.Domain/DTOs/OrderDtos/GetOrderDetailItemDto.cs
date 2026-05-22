namespace ETicaretAPI.Services.Basket.Domain.DTOs.OrderDtos;

public class GetOrderDetailItemDto
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? BrandName { get; set; }
    public string? CategoryName { get; set; }
    public List<OrderProductImageDto> Images { get; set; } = [];
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal TotalNetPrice { get; set; }
}

public class OrderProductImageDto
{
    public int Id { get; set; }
    public string? Url { get; set; }
    public bool IsCover { get; set; }
    public string? AltText { get; set; }
}
