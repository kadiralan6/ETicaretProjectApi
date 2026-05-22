using System.Linq.Expressions;
using ETicaretAPI.Common.Domain.Enums;
using ETicaretAPI.Services.Basket.Domain.Entities;
using ETicaretAPI.Services.Basket.Domain.Enums;

namespace ETicaretAPI.Services.Basket.Application.Orders;

public static class CouponOrder
{
    public static List<(Expression<Func<Coupon, object>> orderSelector, bool orderAsc)> GetOrder(
        CouponOrderByEnum orderBy, OrderTypeEnum orderType)
    {
        var orders = new List<(Expression<Func<Coupon, object>> orderSelector, bool orderAsc)>();

        Expression<Func<Coupon, object>> orderSelector;
        Expression<Func<Coupon, object>> createdAtSelector = x => x.CreatedAt;

        switch (orderBy)
        {
            case CouponOrderByEnum.ModifiedAt:
                orderSelector = x => x.ModifiedAt!;
                break;
            case CouponOrderByEnum.ExpirationDate:
                orderSelector = x => x.ExpirationDate;
                break;
            case CouponOrderByEnum.Code:
                orderSelector = x => x.Code;
                break;
            default:
                orderSelector = x => x.CreatedAt;
                break;
        }

        orders.Add((orderSelector, orderType == OrderTypeEnum.ASC));

        if (orderBy != CouponOrderByEnum.CreatedAt)
            orders.Add((createdAtSelector, orderType == OrderTypeEnum.ASC));

        return orders;
    }
}
