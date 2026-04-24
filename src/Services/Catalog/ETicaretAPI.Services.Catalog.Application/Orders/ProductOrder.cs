using System.Linq.Expressions;
using ETicaretAPI.Common.Domain.Enums;
using ETicaretAPI.Services.Catalog.Domain.Entities;
using ETicaretAPI.Services.Catalog.Domain.Enums;

namespace ETicaretAPI.Services.Catalog.Application.Orders;

public static class ProductOrder
{
    public static List<(Expression<Func<Product, object>> orderSelector, bool orderAsc)> GetOrder(ProductOrderByEnum orderBy, OrderTypeEnum orderType)
    {
        var orders = new List<(Expression<Func<Product, object>> orderSelector, bool orderAsc)>();

        Expression<Func<Product, object>> orderSelector;
        Expression<Func<Product, object>> createdAtSelector = x => x.CreatedAt;

        switch (orderBy)
        {
            case ProductOrderByEnum.ModifiedAt:
                orderSelector = x => x.ModifiedAt;
                break;
            case ProductOrderByEnum.Name:
                orderSelector = x => x.Name;
                break;
            case ProductOrderByEnum.Price:
                orderSelector = x => x.Price;
                break;
            case ProductOrderByEnum.StockQuantity:
                orderSelector = x => x.StockQuantity;
                break;
            default:
                orderSelector = x => x.CreatedAt;
                break;
        }

        orders.Add((orderSelector, orderType == OrderTypeEnum.ASC));

        if (orderBy != ProductOrderByEnum.CreatedAt)
        {
            orders.Add((createdAtSelector, orderType == OrderTypeEnum.ASC));
        }

        return orders;
    }
}
