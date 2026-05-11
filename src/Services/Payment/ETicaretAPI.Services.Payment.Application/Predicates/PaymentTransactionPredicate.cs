using System.Linq.Expressions;
using ETicaretAPI.Common.Infrastructure.Extensions;
using ETicaretAPI.Services.Payment.Domain.DTOs.PaymentTransactionDtos;
using ETicaretAPI.Services.Payment.Domain.Entities;

namespace ETicaretAPI.Services.Payment.Application.Predicates;

public static class PaymentTransactionPredicate
{
    public static Expression<Func<PaymentTransaction, bool>> GetExpression(GetPaymentTransactionForAdminFilterDto filterDto)
    {
        var predicate = PredicateBuilder.True<PaymentTransaction>();

        if (filterDto.OrderId.HasValue)
            predicate = predicate.And(x => x.OrderId == filterDto.OrderId.Value);

        if (!string.IsNullOrEmpty(filterDto.UserId))
            predicate = predicate.And(x => x.UserId == filterDto.UserId);

        if (!string.IsNullOrEmpty(filterDto.TransactionId))
            predicate = predicate.And(x => x.TransactionId.Contains(filterDto.TransactionId));

        if (!string.IsNullOrEmpty(filterDto.Currency))
            predicate = predicate.And(x => x.Currency == filterDto.Currency);

        if (filterDto.Method.HasValue)
            predicate = predicate.And(x => x.Method == filterDto.Method.Value);

        if (filterDto.Status.HasValue)
            predicate = predicate.And(x => x.Status == filterDto.Status.Value);

        if (filterDto.MinAmount.HasValue)
            predicate = predicate.And(x => x.Amount >= filterDto.MinAmount.Value);

        if (filterDto.MaxAmount.HasValue)
            predicate = predicate.And(x => x.Amount <= filterDto.MaxAmount.Value);

        return predicate;
    }
}
