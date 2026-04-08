using ETicaretAPI.Services.Payment.Domain.Entities;

namespace ETicaretAPI.Services.Payment.Application.DTOs;

public class OrderDto
{
  public int Id { get; set; }
  public int UserId { get; set; }
  public string OrderNumber { get; set; } = string.Empty;
  public OrderStatus Status { get; set; }
  public string StatusText => Status.ToString();
  public decimal TotalPrice { get; set; }
  public decimal? DiscountAmount { get; set; }
  public decimal FinalPrice { get; set; }
  public string? Note { get; set; }
  public string ShippingAddress { get; set; } = string.Empty;
  public string ShippingCity { get; set; } = string.Empty;
  public DateTime CreatedDate { get; set; }
  public List<OrderItemDto> Items { get; set; } = new();
  public PaymentTransactionDto? Payment { get; set; }
}

public class OrderItemDto
{
  public int ProductId { get; set; }
  public string ProductName { get; set; } = string.Empty;
  public string? ProductImageUrl { get; set; }
  public int Quantity { get; set; }
  public decimal UnitPrice { get; set; }
  public decimal TotalPrice { get; set; }
}

public class CreateOrderDto
{
  public int UserId { get; set; }
  public List<CreateOrderItemDto> Items { get; set; } = new();
  public string ShippingAddress { get; set; } = string.Empty;
  public string ShippingCity { get; set; } = string.Empty;
  public string ShippingFullName { get; set; } = string.Empty;
  public string ShippingPhoneNumber { get; set; } = string.Empty;
  public string? Note { get; set; }
  public string? CouponCode { get; set; }
}

public class CreateOrderItemDto
{
  public int ProductId { get; set; }
  public string ProductName { get; set; } = string.Empty;
  public string? ProductImageUrl { get; set; }
  public int Quantity { get; set; }
  public decimal UnitPrice { get; set; }
}

public class ProcessPaymentDto
{
  public int OrderId { get; set; }
  public PaymentMethod Method { get; set; }
  public string? CardNumber { get; set; }
  public string? CardHolderName { get; set; }
  public string? ExpiryMonth { get; set; }
  public string? ExpiryYear { get; set; }
  public string? CVV { get; set; }
}

public class PaymentTransactionDto
{
  public string TransactionId { get; set; } = string.Empty;
  public PaymentMethod Method { get; set; }
  public PaymentStatus Status { get; set; }
  public decimal Amount { get; set; }
  public DateTime? CompletedDate { get; set; }
}

public class UpdateOrderStatusDto
{
  public int OrderId { get; set; }
  public OrderStatus NewStatus { get; set; }
}
