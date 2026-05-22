using ETicaretAPI.Common.Persistence.DataAccess.EntityFramework;
using ETicaretAPI.Services.Basket.Application.Repositories;
using ETicaretAPI.Services.Basket.Domain.Entities;
using ETicaretAPI.Services.Basket.Persistence.Context;

namespace ETicaretAPI.Services.Basket.Persistence.Repositories;

public class OrderItemRepository : EfEntityRepositoryBase<OrderItem, BasketDbContext>, IOrderItemRepository
{
    public OrderItemRepository(BasketDbContext context) : base(context)
    {
    }
}