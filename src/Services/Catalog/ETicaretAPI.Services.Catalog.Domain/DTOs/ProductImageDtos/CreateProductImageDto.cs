using Microsoft.AspNetCore.Http;

namespace ETicaretAPI.Services.Catalog.Domain.DTOs.ProductImageDtos;

public class CreateProductImageDto
{
    public List<IFormFile?> Files { get; set; } = new List<IFormFile?>();
    public bool IsCover { get; set; }
    public int ProductId { get; set; }
}
