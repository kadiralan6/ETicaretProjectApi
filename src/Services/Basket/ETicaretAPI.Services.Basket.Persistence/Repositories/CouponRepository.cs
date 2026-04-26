using ETicaretAPI.Common.Persistence.DataAccess.EntityFramework;
using ETicaretAPI.Services.Basket.Application.Repositories;
using ETicaretAPI.Services.Basket.Domain.Entities;
using ETicaretAPI.Services.Basket.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Services.Basket.Persistence.Repositories;

public class CouponRepository : EfEntityRepositoryBase<Coupon, BasketDbContext>, ICouponRepository
{
    public CouponRepository(BasketDbContext context) : base(context)
    {
    }

    public async Task<bool> IsCodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Coupons
            .AsNoTracking()
            .Where(c => c.Code == code && !c.IsDeleted);

        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<Coupon?> GetValidCouponByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Coupons
            .AsNoTracking()
            .FirstOrDefaultAsync(
                c => c.Code == code
                     && c.IsActive
                     && !c.IsDeleted
                     && c.ExpirationDate > DateTime.UtcNow,
                cancellationToken);
    }
}
