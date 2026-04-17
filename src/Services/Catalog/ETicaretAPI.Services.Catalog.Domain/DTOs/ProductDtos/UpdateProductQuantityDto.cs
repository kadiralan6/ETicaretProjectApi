using ETicaretAPI.Common.Domain.Entities;

namespace ETicaretAPI.Services.Catalog.Domain.DTOs.ProductDtos;

public class UpdateProductQuantityDto : BaseEmptyDto
{
    public int? ProductId { get; set; }
    public int? Quantity { get; set; }
}