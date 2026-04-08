using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Services.Catalog.Domain.DTOs.ProductDtos;

namespace ETicaretAPI.Services.Catalog.Application.Services.ProductService;

public interface IProductService
{
  Task<ApiResponse<PagedResult<GetProductDto>>> GetProductsFilterAsync(GetProductForAdminFilterDto filterDto, CancellationToken cancellationToken = default);
  Task<ApiResponse<GetProductDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
  Task<ApiResponse<GetProductDto>> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default);
  Task<ApiResponse<GetProductDto>> UpdateAsync(UpdateProductDto dto, CancellationToken cancellationToken = default);
  Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
  Task<ApiResponse<bool>> UpdateStockAsync(int productId, int quantity, CancellationToken cancellationToken = default);
}
