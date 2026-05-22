using ETicaretAPI.Common.Persistence.DataAccess.EntityFramework;
using ETicaretAPI.Services.Basket.Application.Repositories;
using ETicaretAPI.Services.Basket.Domain.Entities;
using ETicaretAPI.Services.Basket.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Services.Basket.Persistence.Repositories;

public class CartItemsRepository : EfEntityRepositoryBase<CartItems, BasketDbContext>, ICartItemsRepository
{
    public CartItemsRepository(BasketDbContext context) : base(context)
    {
    }

    public async Task<List<CartItems>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.CartItems
            .AsNoTracking()
            .Where(x => x.UserId == userId && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<CartItems?> GetByUserAndProductAsync(int userId, int productId, CancellationToken cancellationToken = default)
    {
        return await _context.CartItems
            .FirstOrDefaultAsync(
                x => x.UserId == userId && x.ProductId == productId && !x.IsDeleted,
                cancellationToken);
    }

    public async Task<(int TotalQuantity, int UniqueItemCount)> GetItemCountByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var query = _context.CartItems
            .AsNoTracking()
            .Where(x => x.UserId == userId && !x.IsDeleted);

        var totalQuantity = await query.SumAsync(x => (int?)x.Quantity, cancellationToken) ?? 0;
        var uniqueItemCount = await query.CountAsync(cancellationToken);

        return (totalQuantity, uniqueItemCount);
    }
}