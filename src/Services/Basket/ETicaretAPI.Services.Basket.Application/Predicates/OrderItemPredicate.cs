using System.Linq.Expressions;
using ETicaretAPI.Common.Infrastructure.Extensions;
using ETicaretAPI.Services.Basket.Domain.DTOs.OrderItemDtos;
using ETicaretAPI.Services.Basket.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Application.Predicates;

public static class OrderItemPredicate
{
    public static Expression<Func<OrderItem, bool>> GetExpression(GetOrderItemForAdminFilterDto filterDto)
    {
        var predicate = PredicateBuilder.True<OrderItem>();

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