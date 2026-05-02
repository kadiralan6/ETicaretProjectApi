using System.Linq.Expressions;
using ETicaretAPI.Common.Domain.Enums;
using ETicaretAPI.Services.Basket.Domain.Entities;
using ETicaretAPI.Services.Basket.Domain.Enums;

namespace ETicaretAPI.Services.Basket.Application.Orders;

public static class OrderOrder
{
    public static List<(Expression<Func<Order, object>> orderSelector, bool orderAsc)> GetOrder(
        OrderOrderByEnum orderBy, OrderTypeEnum orderType)
    {
        var orders = new List<(Expression<Func<Order, object>> orderSelector, bool orderAsc)>();

        Expression<Func<Order, object>> orderSelector;
        Expression<Func<Order, object>> createdAtSelector = x => x.CreatedAt;

        switch (orderBy)
        {
            case OrderOrderByEnum.ModifiedAt:
                orderSelector = x => x.ModifiedAt!;
                break;
            case OrderOrderByEnum.OrderNumber:
                orderSelector = x => x.OrderNumber!;
                break;
            case OrderOrderByEnum.TotalPrice:
                orderSelector = x => x.TotalPrice;
                break;
            default:
                orderSelector = x => x.CreatedAt;
                break;
        }

        orders.Add((orderSelector, orderType == OrderTypeEnum.ASC));

        if (orderBy != OrderOrderByEnum.CreatedAt)
            orders.Add((createdAtSelector, orderType == OrderTypeEnum.ASC));

        return orders;
    }
}