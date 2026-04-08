using ETicaretAPI.Common.Domain.Entities;

namespace ETicaretAPI.Services.Catalog.Domain.Entities;


public class Brand : Entity<int>
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Slug { get; set; }
    public bool? IsActive { get; set; }
    public ICollection<Product> Products { get; set; }
}
