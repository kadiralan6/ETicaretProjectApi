using System.Linq.Expressions;
using ETicaretAPI.Common.Domain.Enums;
using ETicaretAPI.Services.Catalog.Domain.Entities;
using ETicaretAPI.Services.Catalog.Domain.Enums;

namespace ETicaretAPI.Services.Catalog.Application.Orders;

public static class BrandOrder
{
  public static List<(Expression<Func<Brand, object>> orderSelector, bool orderAsc)> GetOrder(BrandOrderByEnum orderBy, OrderTypeEnum orderType)
  {
    var orders = new List<(Expression<Func<Brand, object>> orderSelector, bool orderAsc)>();

    Expression<Func<Brand, object>> orderSelector;
    Expression<Func<Brand, object>> createdAtSelector = x => x.CreatedAt;

    switch (orderBy)
    {
      case BrandOrderByEnum.ModifiedAt:
        orderSelector = x => x.ModifiedAt;
        break;
      case BrandOrderByEnum.Name:
        orderSelector = x => x.Name;
        break;
      default:
        orderSelector = x => x.CreatedAt;
        break;
    }

    orders.Add((orderSelector, orderType == OrderTypeEnum.ASC));

    if (orderBy != BrandOrderByEnum.CreatedAt)
    {
      orders.Add((createdAtSelector, orderType == OrderTypeEnum.ASC));
    }

    return orders;
  }
}
