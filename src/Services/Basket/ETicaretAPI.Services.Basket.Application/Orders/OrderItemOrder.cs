using System.Linq.Expressions;
using ETicaretAPI.Common.Domain.Enums;
using ETicaretAPI.Services.Basket.Domain.Entities;
using ETicaretAPI.Services.Basket.Domain.Enums;

namespace ETicaretAPI.Services.Basket.Application.Orders;

public static class OrderItemOrder
{
    public static List<(Expression<Func<OrderItem, object>> orderSelector, bool orderAsc)> GetOrder(
        OrderItemOrderByEnum orderBy, OrderTypeEnum orderType)
    {
        var orders = new List<(Expression<Func<OrderItem, object>> orderSelector, bool orderAsc)>();

        Expression<Func<OrderItem, object>> orderSelector;
        Expression<Func<OrderItem, object>> createdAtSelector = x => x.CreatedAt;

        switch (orderBy)
        {
            case OrderItemOrderByEnum.ModifiedAt:
                orderSelector = x => x.ModifiedAt!;
                break;
            case OrderItemOrderByEnum.OrderNumber:
                orderSelector = x => x.OrderNumber;
                break;
            case OrderItemOrderByEnum.TotalPrice:
                orderSelector = x => x.TotalPrice!;
                break;
            default:
                orderSelector = x => x.CreatedAt;
                break;
        }

        orders.Add((orderSelector, orderType == OrderTypeEnum.ASC));

        if (orderBy != OrderItemOrderByEnum.CreatedAt)
            orders.Add((createdAtSelector, orderType == OrderTypeEnum.ASC));

        return orders;
    }
}