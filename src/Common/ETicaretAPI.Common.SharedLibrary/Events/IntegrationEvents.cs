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

/// <summary>
/// Ürün oluşturulduğunda fırlatılan event.
/// Catalog -> Search servisine iletilir (Elasticsearch indexleme).
/// </summary>
public class ProductCreatedEvent
{
  public int ProductId { get; set; }
  public string? Code { get; set; }
  public string? Name { get; set; }
  public string? Description { get; set; }
  public string? Slug { get; set; }
  public decimal Price { get; set; }
  public int StockQuantity { get; set; }
  public bool IsActive { get; set; }
  public bool IsFeatured { get; set; }
  public int? CategoryId { get; set; }
  public string? CategoryName { get; set; }
  public int? BrandId { get; set; }
  public string? BrandName { get; set; }
  public List<string> ImageUrls { get; set; } = new();
  public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Ürün güncellendiğinde fırlatılan event.
/// Catalog -> Search servisine iletilir (Elasticsearch güncelleme).
/// </summary>
public class ProductUpdatedEvent
{
  public int ProductId { get; set; }
  public string? Code { get; set; }
  public string? Name { get; set; }
  public string? Description { get; set; }
  public string? Slug { get; set; }
  public decimal Price { get; set; }
  public int StockQuantity { get; set; }
  public bool IsActive { get; set; }
  public bool IsFeatured { get; set; }
  public int? CategoryId { get; set; }
  public string? CategoryName { get; set; }
  public int? BrandId { get; set; }
  public string? BrandName { get; set; }
  public List<string> ImageUrls { get; set; } = new();
  public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Ürün silindiğinde fırlatılan event.
/// Catalog -> Search servisine iletilir (Elasticsearch'ten silme).
/// </summary>
public class ProductDeletedEvent
{
  public int ProductId { get; set; }
  public DateTime DeletedDate { get; set; } = DateTime.UtcNow;
}
