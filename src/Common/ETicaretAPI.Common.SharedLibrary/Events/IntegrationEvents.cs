namespace ETicaretAPI.Common.SharedLibrary.Events;

/// <summary>
/// Sipariş oluşturulduğunda fırlatılan event.
/// Basket -> Payment servisine iletilir.
/// </summary>
public class OrderCreatedEvent
{
  public int OrderId { get; set; }
  public int UserId { get; set; }
  public decimal TotalPrice { get; set; }
  public List<OrderItemEvent> Items { get; set; } = new();
  public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}

public class OrderItemEvent
{
  public int ProductId { get; set; }
  public string ProductName { get; set; } = string.Empty;
  public int Quantity { get; set; }
  public decimal UnitPrice { get; set; }
}

/// <summary>
/// Ödeme tamamlandığında fırlatılan event.
/// Payment -> Catalog (stok güncelleme) ve Basket (sepet temizleme) servislerine iletilir.
/// </summary>
public class PaymentCompletedEvent
{
  public int OrderId { get; set; }
  public int UserId { get; set; }
  public decimal Amount { get; set; }
  public string TransactionId { get; set; } = string.Empty;
  public DateTime CompletedDate { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Ödeme başarısız olduğunda fırlatılan event.
/// </summary>
public class PaymentFailedEvent
{
  public int OrderId { get; set; }
  public int UserId { get; set; }
  public string Reason { get; set; } = string.Empty;
  public DateTime FailedDate { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Stok güncellendiğinde fırlatılan event.
/// </summary>
public class StockUpdatedEvent
{
  public int ProductId { get; set; }
  public int NewStockQuantity { get; set; }
  public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
}
