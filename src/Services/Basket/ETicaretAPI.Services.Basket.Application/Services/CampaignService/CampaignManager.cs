using AutoMapper;
using ETicaretAPI.Common.Application.Exceptions;
using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Services.Basket.Application.Orders;
using ETicaretAPI.Services.Basket.Application.Predicates;
using ETicaretAPI.Services.Basket.Application.Repositories;
using ETicaretAPI.Services.Basket.Domain.DTOs.CampaignDtos;
using ETicaretAPI.Services.Basket.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Application.Services.CampaignService;

public class CampaignManager : ICampaignService
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CampaignManager(
        ICampaignRepository campaignRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _campaignRepository = campaignRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResult<GetCampaignDto>>> GetCampaignsFilterAsync(
        GetCampaignForAdminFilterDto filterDto, CancellationToken cancellationToken = default)
    {
        var predicate = CampaignPredicate.GetExpression(filterDto);
        var orders = CampaignOrder.GetOrder(filterDto.OrderBy, filterDto.OrderType);

        var campaigns = await _campaignRepository.GetAllAsNoTrackingWithPaginationAsync(
            filterDto.Page, filterDto.PageSize, predicate, orders, cancellationToken: cancellationToken);

        var result = _mapper.Map<PagedResult<GetCampaignDto>>(campaigns);
        return ApiResponse<PagedResult<GetCampaignDto>>.Success(result);
    }

    public async Task<ApiResponse<GetCampaignDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var campaign = await _campaignRepository.GetWithAsNoTrackingAsync(
            x => x.Id == id, cancellationToken: cancellationToken);

        if (campaign is null)
            throw new NotFoundException(nameof(Campaign), id);

        var result = _mapper.Map<GetCampaignDto>(campaign);
        return ApiResponse<GetCampaignDto>.Success(result);
    }

    public async Task<ApiResponse<GetCampaignDto>> CreateAsync(CreateCampaignDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.StartDate >= dto.EndDate)
            throw new ValidationException(["Başlangıç tarihi bitiş tarihinden önce olmalıdır."]);

        if (dto.EndDate <= DateTime.UtcNow)
            throw new ValidationException(["Bitiş tarihi geçmiş bir kampanya oluşturulamaz."]);

        var campaign = _mapper.Map<Campaign>(dto);
        campaign.IsActive = true;
        campaign.UsageCount = 0;
        campaign.IsValid = dto.EndDate > DateTime.UtcNow;

        await _campaignRepository.AddAsync(campaign, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<GetCampaignDto>(campaign);
        return ApiResponse<GetCampaignDto>.Success(result);
    }

    public async Task<ApiResponse<GetCampaignDto>> UpdateAsync(UpdateCampaignDto dto, CancellationToken cancellationToken = default)
    {
        var campaign = await _campaignRepository.GetAsync(x => x.Id == dto.Id, cancellationToken: cancellationToken);

        if (campaign is null)
            throw new NotFoundException(nameof(Campaign), dto.Id);

        if (dto.StartDate >= dto.EndDate)
            throw new ValidationException(["Başlangıç tarihi bitiş tarihinden önce olmalıdır."]);

        _mapper.Map(dto, campaign);
        campaign.IsValid = campaign.EndDate > DateTime.UtcNow;

        await _campaignRepository.UpdateAsync(campaign, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<GetCampaignDto>(campaign);
        return ApiResponse<GetCampaignDto>.Success(result);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var campaign = await _campaignRepository.GetAsync(x => x.Id == id, cancellationToken: cancellationToken);

        if (campaign is null)
            throw new NotFoundException(nameof(Campaign), id);

        await _campaignRepository.SoftDeleteAsync(campaign, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Success(true);
    }
}
