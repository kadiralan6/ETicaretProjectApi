using System.Linq.Expressions;
using ETicaretAPI.Common.Domain.Enums;
using ETicaretAPI.Services.Catalog.Domain.Entities;
using ETicaretAPI.Services.Catalog.Domain.Enums;

namespace ETicaretAPI.Services.Catalog.Application.Orders;

public static class FeaturedProductOrder
{
    public static List<(Expression<Func<Product, object>> orderSelector, bool orderAsc)> GetOrder(
        FeaturedProductOrderByEnum orderBy, OrderTypeEnum orderType)
    {
        var orders = new List<(Expression<Func<Product, object>> orderSelector, bool orderAsc)>();

        Expression<Func<Product, object>> createdAtSelector = x => x.CreatedAt;

        Expression<Func<Product, object>> orderSelector = orderBy switch
        {
            FeaturedProductOrderByEnum.Name  => x => x.Name!,
            FeaturedProductOrderByEnum.Price => x => x.Price,
            _                                => x => x.CreatedAt
        };

        orders.Add((orderSelector, orderType == OrderTypeEnum.ASC));

        if (orderBy != FeaturedProductOrderByEnum.CreatedAt)
            orders.Add((createdAtSelector, orderType == OrderTypeEnum.ASC));

        return orders;
    }
}
