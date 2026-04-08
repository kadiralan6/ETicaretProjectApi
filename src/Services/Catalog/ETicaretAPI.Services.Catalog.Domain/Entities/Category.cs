using ETicaretAPI.Common.Domain.Entities;

namespace ETicaretAPI.Services.Catalog.Domain.Entities;

public class Category : Entity<int>
{
    public string? Description { get; set; }
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public string? ImageUrl { get; set; }
    public int? ParentCategoryId { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }

    // Navigation properties
    public Category? ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; }
}
