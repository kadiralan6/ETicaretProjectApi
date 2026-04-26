using System.Linq.Expressions;
using ETicaretAPI.Common.Infrastructure.Extensions;
using ETicaretAPI.Services.Basket.Domain.DTOs.CartDtos;
using ETicaretAPI.Services.Basket.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Application.Predicates;

public static class CartPredicate
{
    public static Expression<Func<Cart, bool>> GetExpression(GetCartForAdminFilterDto filterDto)
    {
        var predicate = PredicateBuilder.True<Cart>();

        if (filterDto.UserId.HasValue)
            predicate = predicate.And(c => c.UserId == filterDto.UserId.Value);

        if (filterDto.CouponId.HasValue)
            predicate = predicate.And(c => c.CouponId == filterDto.CouponId.Value);

        if (filterDto.MinTotal.HasValue)
            predicate = predicate.And(c => c.Total >= filterDto.MinTotal.Value);

        if (filterDto.MaxTotal.HasValue)
            predicate = predicate.And(c => c.Total <= filterDto.MaxTotal.Value);

        return predicate;
    }
}
