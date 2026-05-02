using ETicaretAPI.Common.Persistence.DataAccess.EntityFramework;
using ETicaretAPI.Services.Basket.Application.Repositories;
using ETicaretAPI.Services.Basket.Domain.Entities;
using ETicaretAPI.Services.Basket.Persistence.Context;

namespace ETicaretAPI.Services.Basket.Persistence.Repositories;

public class OrderRepository : EfEntityRepositoryBase<Order, BasketDbContext>, IOrderRepository
{
    public OrderRepository(BasketDbContext context) : base(context)
    {
    }
}