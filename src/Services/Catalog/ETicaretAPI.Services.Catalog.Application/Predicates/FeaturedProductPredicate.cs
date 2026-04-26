using System.Linq.Expressions;
using ETicaretAPI.Common.Infrastructure.Extensions;
using ETicaretAPI.Services.Catalog.Domain.DTOs.StorefrontDtos;
using ETicaretAPI.Services.Catalog.Domain.Entities;

namespace ETicaretAPI.Services.Catalog.Application.Predicates;

public static class FeaturedProductPredicate
{
    public static Expression<Func<Product, bool>> GetExpression(GetFeaturedProductsFilterDto filterDto)
    {
        var predicate = PredicateBuilder.True<Product>();

        predicate = predicate.And(p => p.IsFeatured && p.IsActive);

        if (filterDto.CategoryId.HasValue)
            predicate = predicate.And(p => p.CategoryId == filterDto.CategoryId.Value);

        if (filterDto.BrandId.HasValue)
            predicate = predicate.And(p => p.BrandId == filterDto.BrandId.Value);

        if (filterDto.MinPrice.HasValue)
            predicate = predicate.And(p => p.Price >= filterDto.MinPrice.Value);

        if (filterDto.MaxPrice.HasValue)
            predicate = predicate.And(p => p.Price <= filterDto.MaxPrice.Value);

        if (!string.IsNullOrWhiteSpace(filterDto.Search))
            predicate = predicate.And(p =>
                p.Name!.Contains(filterDto.Search) ||
                (p.ShortDescription != null && p.ShortDescription.Contains(filterDto.Search)));

        return predicate;
    }
}
