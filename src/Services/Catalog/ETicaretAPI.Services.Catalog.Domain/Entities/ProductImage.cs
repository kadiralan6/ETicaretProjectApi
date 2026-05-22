using ETicaretAPI.Common.Domain.Entities;

namespace ETicaretAPI.Services.Catalog.Domain.Entities;

public class ProductImage : Entity<int>
{
    public string? Url { get; set; }
    public string? AltText { get; set; }
    public bool IsCover { get; set; }

    public int ProductId { get; set; }

    // Navigation property
    public Product? Product { get; set; }
}
