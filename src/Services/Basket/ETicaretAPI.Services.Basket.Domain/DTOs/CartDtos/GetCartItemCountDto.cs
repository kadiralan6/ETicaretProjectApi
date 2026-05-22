namespace ETicaretAPI.Services.Basket.Domain.DTOs.CartDtos;

public class GetCartItemCountDto
{
    public int UserId { get; set; }

    /// <summary>
    /// Sepetteki toplam ürün adedi (adet bazlı). İkon badge için kullanılır.
    /// Örn: 2 adet iPhone + 3 adet kılıf = 5
    /// </summary>
    public int TotalQuantity { get; set; }

    /// <summary>
    /// Sepetteki farklı ürün sayısı (çeşit bazlı).
    /// Örn: 2 adet iPhone + 3 adet kılıf = 2
    /// </summary>
    public int UniqueItemCount { get; set; }
}
