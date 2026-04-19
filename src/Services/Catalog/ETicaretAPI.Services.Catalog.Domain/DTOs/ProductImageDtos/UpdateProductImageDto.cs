using Microsoft.AspNetCore.Http;
using ETicaretAPI.Common.Domain.Entities;

namespace ETicaretAPI.Services.Catalog.Domain.DTOs.ProductImageDtos;

public class UpdateProductImageDto : BaseDto
{
    public List<IFormFile?> Files { get; set; } = new List<IFormFile?>();
    public bool IsCover { get; set; }
    public int ProductId { get; set; }
}
