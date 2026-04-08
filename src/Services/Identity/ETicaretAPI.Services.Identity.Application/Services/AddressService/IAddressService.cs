using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Services.Identity.Domain.DTOs;

namespace ETicaretAPI.Services.Identity.Application.Services.AddressService;

public interface IAddressService
{
    Task<ApiResponse<List<AddressDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<AddressDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<AddressDto>> CreateAsync(CreateAddressDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<AddressDto>> UpdateAsync(int id, UpdateAddressDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
