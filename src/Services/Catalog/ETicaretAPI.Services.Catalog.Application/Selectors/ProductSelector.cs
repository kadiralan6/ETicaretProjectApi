using System.Linq.Expressions;
using ETicaretAPI.Services.Catalog.Domain.Entities;

namespace ETicaretAPI.Services.Catalog.Application.Selectors;

public static class ProductSelector
{
    public static Expression<Func<Product, object>>[] GetProductIncludes() =>
    [
        x => x.Category,
        x => x.Brand
    ];

    public static Expression<Func<Product, Product>> GetProductWithDetailsSelector()
    {
        return x => new Product
        {
            Id = x.Id,
            Code = x.Code,
            Name = x.Name,
            Description = x.Description,
            Slug = x.Slug,
            Price = x.Price,
            StockQuantity = x.StockQuantity,
            IsActive = x.IsActive,
            IsFeatured = x.IsFeatured,
            CategoryId = x.CategoryId,
            BrandId = x.BrandId,
            CreatedAt = x.CreatedAt,
            ModifiedAt = x.ModifiedAt,
            Category = x.Category != null ? new Category { Id = x.Category.Id, Name = x.Category.Name } : null,
            Brand = x.Brand != null ? new Brand { Id = x.Brand.Id, Name = x.Brand.Name } : null
        };
    }
}
