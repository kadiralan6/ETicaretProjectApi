using System.Linq.Expressions;
using ETicaretAPI.Services.Catalog.Domain.Entities;

namespace ETicaretAPI.Services.Catalog.Application.Selectors;


public static class CategorySelector
{
  public static Expression<Func<Category, Category>> GetCategoryWithSubCategoriesSelector()
    {
        return x => new Category
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            Slug = x.Slug,
            ImageUrl = x.ImageUrl,
            ParentCategoryId = x.ParentCategoryId,
            DisplayOrder = x.DisplayOrder,
            IsActive = x.IsActive,
            CreatedAt = x.CreatedAt,
            ModifiedAt = x.ModifiedAt,
            SubCategories = x.SubCategories.Select(sub => new Category
            {
                Id = sub.Id,
                Name = sub.Name,
                Description = sub.Description,
                Slug = sub.Slug,
                ImageUrl = sub.ImageUrl,
                ParentCategoryId = sub.ParentCategoryId,
                DisplayOrder = sub.DisplayOrder,
                IsActive = sub.IsActive,
                CreatedAt = sub.CreatedAt,
                ModifiedAt = sub.ModifiedAt
            }).ToList()
        };
    }
}