using System.Linq.Expressions;
using ETicaretAPI.Common.Infrastructure.Extensions;
using ETicaretAPI.Services.Catalog.Domain.DTOs.BrandDtos;
using ETicaretAPI.Services.Catalog.Domain.Entities;

namespace ETicaretAPI.Services.Catalog.Application.Predicates;

public static class BrandPredicate
{
  public static Expression<Func<Brand, bool>> GetExpression(GetBrandForAdminFilterDto filterDto)
  {
    var predicate = PredicateBuilder.True<Brand>();

    if (!string.IsNullOrEmpty(filterDto.Name))
      predicate = predicate.And(c => c.Name.Contains(filterDto.Name));

    if (filterDto.IsActive.HasValue)
      predicate = predicate.And(c => c.IsActive == filterDto.IsActive.Value);

    return predicate;
  }
}
