using System;
using System.Linq.Expressions;
using ETicaretAPI.Common.Domain.Enums;
using ETicaretAPI.Services.Catalog.Domain.Entities;
using ETicaretAPI.Services.Catalog.Domain.Enums;

namespace ETicaretAPI.Services.Catalog.Application.Orders;

public static class CategoryOrder
{
    public static List<(Expression<Func<Category, object>> orderSelector, bool orderAsc)> GetOrder(CategoryOrderByEnum orderBy, OrderTypeEnum orderType)
    {
        var orders = new List<(Expression<Func<Category, object>> orderSelector, bool orderAsc)>();

        Expression<Func<Category, object>> orderSelector;
        Expression<Func<Category, object>> createdAtSelector = x => x.CreatedAt;

        switch (orderBy)
        {
            case CategoryOrderByEnum.ModifiedAt:
                orderSelector = x => x.ModifiedAt;
                break;
            case CategoryOrderByEnum.Name:
                orderSelector = x => x.Name;
                break;
            default:
                orderSelector = x => x.CreatedAt;
                break;
        }

        orders.Add((orderSelector, orderType == OrderTypeEnum.ASC));

        if (orderBy != CategoryOrderByEnum.CreatedAt)
        {
            orders.Add((createdAtSelector, orderType == OrderTypeEnum.ASC));
        }

        return orders;
    }
}
