using System.Linq.Expressions;
using ETicaretAPI.Common.Infrastructure.Extensions;
using ETicaretAPI.Services.Catalog.Domain.DTOs.CategoryDtos;
using ETicaretAPI.Services.Catalog.Domain.Entities;

namespace ETicaretAPI.Services.Catalog.Application.Predicates;

public static class CategoryPredicate
{
    public static Expression<Func<Category, bool>> GetExpression(GetCategoryForAdminFilterDto filterDto)
    {
        var predicate = PredicateBuilder.True<Category>();

        if (!string.IsNullOrEmpty(filterDto.Name))
            predicate = predicate.And(c => c.Name.Contains(filterDto.Name));

        if (filterDto.IsActive.HasValue)
            predicate = predicate.And(c => c.IsActive == filterDto.IsActive.Value);

        return predicate;

    }
}