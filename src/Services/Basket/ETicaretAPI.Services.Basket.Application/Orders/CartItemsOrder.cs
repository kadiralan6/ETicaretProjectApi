using System.Linq.Expressions;
using ETicaretAPI.Common.Domain.Enums;
using ETicaretAPI.Services.Basket.Domain.Entities;
using ETicaretAPI.Services.Basket.Domain.Enums;

namespace ETicaretAPI.Services.Basket.Application.Orders;

public static class CartItemsOrder
{
    public static List<(Expression<Func<CartItems, object>> orderSelector, bool orderAsc)> GetOrder(
        CartItemsOrderByEnum orderBy, OrderTypeEnum orderType)
    {
        var orders = new List<(Expression<Func<CartItems, object>> orderSelector, bool orderAsc)>();

        Expression<Func<CartItems, object>> orderSelector;
        Expression<Func<CartItems, object>> createdAtSelector = x => x.CreatedAt;

        switch (orderBy)
        {
            case CartItemsOrderByEnum.ModifiedAt:
                orderSelector = x => x.ModifiedAt!;
                break;
            case CartItemsOrderByEnum.Quantity:
                orderSelector = x => x.Quantity;
                break;
            default:
                orderSelector = x => x.CreatedAt;
                break;
        }

        orders.Add((orderSelector, orderType == OrderTypeEnum.ASC));

        if (orderBy != CartItemsOrderByEnum.CreatedAt)
            orders.Add((createdAtSelector, orderType == OrderTypeEnum.ASC));

        return orders;
    }
}
