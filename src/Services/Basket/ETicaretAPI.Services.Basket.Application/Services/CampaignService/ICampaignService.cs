using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Services.Basket.Domain.DTOs.CampaignDtos;

namespace ETicaretAPI.Services.Basket.Application.Services.CampaignService;

public interface ICampaignService
{
    Task<ApiResponse<PagedResult<GetCampaignDto>>> GetCampaignsFilterAsync(GetCampaignForAdminFilterDto filterDto, CancellationToken cancellationToken = default);
    Task<ApiResponse<GetCampaignDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<GetCampaignDto>> CreateAsync(CreateCampaignDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<GetCampaignDto>> UpdateAsync(UpdateCampaignDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
