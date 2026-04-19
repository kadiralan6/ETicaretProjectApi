using System;

namespace ETicaretAPI.Common.Application.DTOs.CatalogDtos;

public class GetProductImageDto
{
    public int Id { get; set; }
    public string? Url { get; set; }
    public bool IsCover { get; set; }
    public int ProductId { get; set; }
}
