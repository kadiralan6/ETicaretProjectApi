using System.Linq.Expressions;
using ETicaretAPI.Services.Catalog.Domain.Entities;

namespace ETicaretAPI.Services.Catalog.Application.Selectors;

public static class FeaturedProductSelector
{
    public static Expression<Func<Product, Product>> GetCardSelector()
    {
        return x => new Product
        {
            Id = x.Id,
            Name = x.Name,
            Slug = x.Slug,
            ShortDescription = x.ShortDescription,
            Price = x.Price,
            StockQuantity = x.StockQuantity,
            IsActive = x.IsActive,
            IsFeatured = x.IsFeatured,
            CategoryId = x.CategoryId,
            BrandId = x.BrandId,
            CreatedAt = x.CreatedAt,
            Category = x.Category != null
                ? new Category
                {
                    Id = x.Category.Id,
                    Name = x.Category.Name,
                    Slug = x.Category.Slug,
                    ParentCategory = x.Category.ParentCategory != null
                        ? new Category
                        {
                            Id = x.Category.ParentCategory.Id,
                            Name = x.Category.ParentCategory.Name,
                            Slug = x.Category.ParentCategory.Slug
                        }
                        : null
                }
                : null,
            Brand = x.Brand != null
                ? new Brand
                {
                    Id = x.Brand.Id,
                    Name = x.Brand.Name,
                    Slug = x.Brand.Slug
                }
                : null
        };
    }
}
