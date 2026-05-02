using System.Linq.Expressions;
using ETicaretAPI.Common.Infrastructure.Extensions;
using ETicaretAPI.Services.Basket.Domain.DTOs.OrderDtos;
using ETicaretAPI.Services.Basket.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Application.Predicates;

public static class OrderPredicate
{
    public static Expression<Func<Order, bool>> GetExpression(GetOrderForAdminFilterDto filterDto)
    {
        var predicate = PredicateBuilder.True<Order>();

        if (filterDto.UserId.HasValue)
            predicate = predicate.And(x => x.UserId == filterDto.UserId.Value);

        if (filterDto.ProductId.HasValue)
            predicate = predicate.And(x => x.ProductId == filterDto.ProductId.Value);

        if (filterDto.CouponId.HasValue)
            predicate = predicate.And(x => x.CouponId == filterDto.CouponId.Value);

        if (!string.IsNullOrWhiteSpace(filterDto.OrderNumber))
            predicate = predicate.And(x => x.OrderNumber == filterDto.OrderNumber);

        return predicate;
    }
}