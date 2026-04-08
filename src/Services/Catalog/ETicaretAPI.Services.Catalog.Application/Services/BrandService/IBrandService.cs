using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Services.Catalog.Domain.DTOs.BrandDtos;

namespace ETicaretAPI.Services.Catalog.Application.Services.BrandService;

public interface IBrandService
{
  Task<ApiResponse<PagedResult<GetBrandDto>>> GetBrandsFilterAsync(GetBrandForAdminFilterDto filterDto, CancellationToken cancellationToken = default);
  Task<ApiResponse<GetBrandDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
  Task<ApiResponse<GetBrandDto>> CreateAsync(CreateBrandDto dto, CancellationToken cancellationToken = default);
  Task<ApiResponse<GetBrandDto>> UpdateAsync(UpdateBrandDto dto, CancellationToken cancellationToken = default);
  Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
