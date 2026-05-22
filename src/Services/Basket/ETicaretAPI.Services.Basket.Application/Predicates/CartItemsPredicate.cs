using System.Linq.Expressions;
using ETicaretAPI.Common.Infrastructure.Extensions;
using ETicaretAPI.Services.Basket.Domain.DTOs.CartDtos;
using ETicaretAPI.Services.Basket.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Application.Predicates;

public static class CartItemsPredicate
{
    public static Expression<Func<CartItems, bool>> GetExpression(GetCartForAdminFilterDto filterDto)
    {
        var predicate = PredicateBuilder.True<CartItems>();

        if (filterDto.UserId.HasValue)
            predicate = predicate.And(c => c.UserId == filterDto.UserId.Value);

        if (filterDto.CouponId.HasValue)
            predicate = predicate.And(c => c.CouponId == filterDto.CouponId.Value);

        if (filterDto.ProductId.HasValue)
            predicate = predicate.And(c => c.ProductId == filterDto.ProductId.Value);

        if (!string.IsNullOrWhiteSpace(filterDto.OrderNumber))
            predicate = predicate.And(c => c.OrderNumber == filterDto.OrderNumber);

        return predicate;
    }
}
