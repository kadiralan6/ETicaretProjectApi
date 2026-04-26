using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Services.Basket.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Application.Repositories;

public interface ICouponRepository : IEntityRepository<Coupon>
{
    /// <summary>
    /// Kupon kodunun başka bir kayıt tarafından kullanılıp kullanılmadığını doğrular.
    /// </summary>
    Task<bool> IsCodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kupon koduna göre aktif ve süresi dolmamış kuponu getirir.
    /// </summary>
    Task<Coupon?> GetValidCouponByCodeAsync(string code, CancellationToken cancellationToken = default);
}
