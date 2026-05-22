using System.Linq.Expressions;
using ETicaretAPI.Common.Domain.Enums;
using ETicaretAPI.Services.Payment.Domain.Entities;
using ETicaretAPI.Services.Payment.Domain.Enums;

namespace ETicaretAPI.Services.Payment.Application.Orders;

public static class PaymentTransactionOrder
{
    public static List<(Expression<Func<PaymentTransaction, object>> orderSelector, bool orderAsc)> GetOrder(PaymentTransactionOrderByEnum orderBy, OrderTypeEnum orderType)
    {
        var orders = new List<(Expression<Func<PaymentTransaction, object>> orderSelector, bool orderAsc)>();

        Expression<Func<PaymentTransaction, object>> orderSelector;
        Expression<Func<PaymentTransaction, object>> createdAtSelector = x => x.CreatedAt;

        switch (orderBy)
        {
            case PaymentTransactionOrderByEnum.ModifiedAt:
                orderSelector = x => x.ModifiedAt;
                break;
            case PaymentTransactionOrderByEnum.Amount:
                orderSelector = x => x.Amount;
                break;
            case PaymentTransactionOrderByEnum.CompletedDate:
                orderSelector = x => x.CompletedDate ?? DateTime.MinValue;
                break;
            default:
                orderSelector = x => x.CreatedAt;
                break;
        }

        orders.Add((orderSelector, orderType == OrderTypeEnum.ASC));

        if (orderBy != PaymentTransactionOrderByEnum.CreatedAt)
        {
            orders.Add((createdAtSelector, orderType == OrderTypeEnum.ASC));
        }

        return orders;
    }
}
