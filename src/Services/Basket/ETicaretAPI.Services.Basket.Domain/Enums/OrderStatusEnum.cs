namespace ETicaretAPI.Services.Basket.Domain.Enums;

public enum OrderStatusEnum
{
    Pending = 0,
    Confirmed = 1,
    PaymentFailed = 2,
    Processing = 3,
    Shipped = 4,
    Delivered = 5,
    Cancelled = 6
}
