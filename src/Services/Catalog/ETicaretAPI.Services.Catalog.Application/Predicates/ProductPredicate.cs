using System.Linq.Expressions;
using ETicaretAPI.Common.Infrastructure.Extensions;
using ETicaretAPI.Services.Catalog.Domain.DTOs.ProductDtos;
using ETicaretAPI.Services.Catalog.Domain.Entities;

namespace ETicaretAPI.Services.Catalog.Application.Predicates;

public static class ProductPredicate
{
  public static Expression<Func<Product, bool>> GetExpression(GetProductForAdminFilterDto filterDto)
  {
    var predicate = PredicateBuilder.True<Product>();

    if (!string.IsNullOrEmpty(filterDto.Name))
      predicate = predicate.And(p => p.Name.Contains(filterDto.Name));

    if (!string.IsNullOrEmpty(filterDto.Code))
      predicate = predicate.And(p => p.Code.Contains(filterDto.Code));

    if (filterDto.IsActive.HasValue)
      predicate = predicate.And(p => p.IsActive == filterDto.IsActive.Value);

    if (filterDto.IsFeatured.HasValue)
      predicate = predicate.And(p => p.IsFeatured == filterDto.IsFeatured.Value);

    if (filterDto.CategoryId.HasValue)
      predicate = predicate.And(p => p.CategoryId == filterDto.CategoryId.Value);

    if (filterDto.BrandId.HasValue)
      predicate = predicate.And(p => p.BrandId == filterDto.BrandId.Value);

    if (filterDto.MinPrice.HasValue)
      predicate = predicate.And(p => p.Price >= filterDto.MinPrice.Value);

    if (filterDto.MaxPrice.HasValue)
      predicate = predicate.And(p => p.Price <= filterDto.MaxPrice.Value);

    return predicate;
  }
}
