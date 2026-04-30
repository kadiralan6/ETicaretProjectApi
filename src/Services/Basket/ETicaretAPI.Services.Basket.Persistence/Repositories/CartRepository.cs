using ETicaretAPI.Common.Persistence.DataAccess.EntityFramework;
using ETicaretAPI.Services.Basket.Application.Repositories;
using ETicaretAPI.Services.Basket.Domain.Entities;
using ETicaretAPI.Services.Basket.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Services.Basket.Persistence.Repositories;

public class CartRepository : EfEntityRepositoryBase<Cart, BasketDbContext>, ICartRepository
{
    public CartRepository(BasketDbContext context) : base(context)
    {
    }

    public async Task<Cart?> GetCartWithItemsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Carts
            .Include(c => c.Items.Where(i => !i.IsDeleted))
            .Include(c => c.Coupon)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted, cancellationToken);
    }

    public async Task<Cart?> GetCartWithItemsAsync(int cartId, CancellationToken cancellationToken = default)
    {
        return await _context.Carts
            .AsNoTracking()
            .Include(c => c.Items.Where(i => !i.IsDeleted))
            .Include(c => c.Coupon)
            .FirstOrDefaultAsync(c => c.Id == cartId && !c.IsDeleted, cancellationToken);
    }

    public async Task<(int TotalQuantity, int UniqueItemCount)> GetItemCountByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var result = await _context.Carts
            .AsNoTracking()
            .Where(c => c.UserId == userId && !c.IsDeleted)
            .Select(c => new
            {
                TotalQuantity = c.Items
                    .Where(i => !i.IsDeleted)
                    .Sum(i => (int?)i.Quantity) ?? 0,
                UniqueItemCount = c.Items
                    .Count(i => !i.IsDeleted)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return result is null ? (0, 0) : (result.TotalQuantity, result.UniqueItemCount);
    }
}
