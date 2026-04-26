using System.Linq.Expressions;
using ETicaretAPI.Common.Infrastructure.Extensions;
using ETicaretAPI.Services.Basket.Domain.DTOs.CouponDtos;
using ETicaretAPI.Services.Basket.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Application.Predicates;

public static class CouponPredicate
{
    public static Expression<Func<Coupon, bool>> GetExpression(GetCouponForAdminFilterDto filterDto)
    {
        var predicate = PredicateBuilder.True<Coupon>();

        if (!string.IsNullOrWhiteSpace(filterDto.Code))
            predicate = predicate.And(c => c.Code.Contains(filterDto.Code));

        if (filterDto.Type.HasValue)
            predicate = predicate.And(c => c.Type == filterDto.Type.Value);

        if (filterDto.IsActive.HasValue)
            predicate = predicate.And(c => c.IsActive == filterDto.IsActive.Value);

        if (filterDto.ExpiresAfter.HasValue)
            predicate = predicate.And(c => c.ExpirationDate >= filterDto.ExpiresAfter.Value);

        return predicate;
    }
}
