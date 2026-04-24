using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.DTOs.CatalogDtos;
using ETicaretAPI.Services.Catalog.Domain.DTOs.ProductImageDtos;

namespace ETicaretAPI.Services.Catalog.Application.Services.ProductImageService;

public interface IProductImageService
{
    Task<ApiResponse<List<GetProductImageDto>>> GetImagesByProductIdAsync(int productId, CancellationToken cancellationToken = default);
    Task<ApiResponse<GetProductImageDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<List<GetProductImageDto>>> CreateAsync(CreateProductImageDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<List<GetProductImageDto>>> UpdateAsync(UpdateProductImageDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
