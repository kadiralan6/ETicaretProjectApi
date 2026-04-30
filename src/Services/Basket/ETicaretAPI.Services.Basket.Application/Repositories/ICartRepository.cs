using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Services.Basket.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Application.Repositories;

public interface ICartRepository : IEntityRepository<Cart>
{
    /// <summary>
    /// Kullanıcıya ait aktif sepeti items ve coupon ile birlikte getirir.
    /// </summary>
    Task<Cart?> GetCartWithItemsByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sepet ID ile sepeti items ve coupon dahil getirir.
    /// </summary>
    Task<Cart?> GetCartWithItemsAsync(int cartId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kullanıcının sepetindeki toplam ürün adedini ve çeşit sayısını döner.
    /// Tam sepet yüklenmez — ikon badge için optimize edilmiş sorgu.
    /// </summary>
    Task<(int TotalQuantity, int UniqueItemCount)> GetItemCountByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}
