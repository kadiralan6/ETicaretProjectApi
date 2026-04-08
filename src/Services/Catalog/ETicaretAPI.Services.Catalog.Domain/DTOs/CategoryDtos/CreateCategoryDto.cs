namespace ETicaretAPI.Services.Catalog.Domain.DTOs.CategoryDtos;

public class CreateCategoryDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int? ParentCategoryId { get; set; }
    public int DisplayOrder { get; set; }
}