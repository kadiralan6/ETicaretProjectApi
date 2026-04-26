using System.Linq.Expressions;
using ETicaretAPI.Common.Domain.Enums;
using ETicaretAPI.Services.Basket.Domain.Entities;
using ETicaretAPI.Services.Basket.Domain.Enums;

namespace ETicaretAPI.Services.Basket.Application.Orders;

public static class CartOrder
{
    public static List<(Expression<Func<Cart, object>> orderSelector, bool orderAsc)> GetOrder(
        CartOrderByEnum orderBy, OrderTypeEnum orderType)
    {
        var orders = new List<(Expression<Func<Cart, object>> orderSelector, bool orderAsc)>();

        Expression<Func<Cart, object>> orderSelector;
        Expression<Func<Cart, object>> createdAtSelector = x => x.CreatedAt;

        switch (orderBy)
        {
            case CartOrderByEnum.ModifiedAt:
                orderSelector = x => x.ModifiedAt!;
                break;
            case CartOrderByEnum.Total:
                orderSelector = x => x.Total;
                break;
            default:
                orderSelector = x => x.CreatedAt;
                break;
        }

        orders.Add((orderSelector, orderType == OrderTypeEnum.ASC));

        if (orderBy != CartOrderByEnum.CreatedAt)
            orders.Add((createdAtSelector, orderType == OrderTypeEnum.ASC));

        return orders;
    }
}
