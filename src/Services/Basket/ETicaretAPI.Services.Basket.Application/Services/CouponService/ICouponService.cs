using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Services.Basket.Domain.DTOs.CouponDtos;

namespace ETicaretAPI.Services.Basket.Application.Services.CouponService;

public interface ICouponService
{
    Task<ApiResponse<PagedResult<GetCouponDto>>> GetCouponsFilterAsync(GetCouponForAdminFilterDto filterDto, CancellationToken cancellationToken = default);
    Task<ApiResponse<GetCouponDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<GetCouponDto>> CreateAsync(CreateCouponDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<GetCouponDto>> UpdateAsync(UpdateCouponDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
