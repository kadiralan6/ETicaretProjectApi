using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Services.Basket.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Application.Repositories;

public interface ICartItemsRepository : IEntityRepository<CartItems>
{
    Task<List<CartItems>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<CartItems?> GetByUserAndProductAsync(int userId, int productId, CancellationToken cancellationToken = default);
    Task<(int TotalQuantity, int UniqueItemCount)> GetItemCountByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}