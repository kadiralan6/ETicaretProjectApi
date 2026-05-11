using ETicaretAPI.Services.Basket.Domain.Enums;

namespace ETicaretAPI.Services.Basket.Domain.DTOs.OrderDtos;

public class PlaceOrderDto
{
    public int CartId { get; set; }
    public int AddressId { get; set; }
    public PaymentMethodEnum PaymentMethod { get; set; }
    public CreditCardInfoDto? CardInfo { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TRY";
    public List<PlaceOrderItemDto> Items { get; set; } = [];
}
