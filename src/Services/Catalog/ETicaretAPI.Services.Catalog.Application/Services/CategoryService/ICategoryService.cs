using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Services.Catalog.Domain.DTOs.CategoryDtos;

namespace ETicaretAPI.Services.Catalog.Application.Services.CategoryService;

public interface ICategoryService
{
    Task<ApiResponse<PagedResult<GetCategoryDto>>> GetCategoriesFilterAsync(GetCategoryForAdminFilterDto filterDto, CancellationToken cancellationToken = default);
    Task<ApiResponse<GetCategoryDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<GetCategoryDto>> GetAdminDetailByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<GetCategoryDto>> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<GetCategoryDto>> UpdateAsync(UpdateCategoryDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
