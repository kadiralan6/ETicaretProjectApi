using ETicaretAPI.Services.Catalog.Domain.Enums;

namespace ETicaretAPI.Services.Catalog.Domain.DTOs.CategoryDtos;

public class GetCategoryForAdminFilterDto : BaseFilterDto<CategoryOrderByEnum>
{
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
}

