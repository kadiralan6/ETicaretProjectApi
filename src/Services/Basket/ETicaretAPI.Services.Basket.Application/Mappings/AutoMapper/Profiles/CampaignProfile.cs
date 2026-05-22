using AutoMapper;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Services.Basket.Domain.DTOs.CampaignDtos;
using ETicaretAPI.Services.Basket.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Application.Mappings.AutoMapper.Profiles;

public class CampaignProfile : Profile
{
    public CampaignProfile()
    {
        CreateMap<Campaign, GetCampaignDto>();
        CreateMap<CreateCampaignDto, Campaign>();
        CreateMap<UpdateCampaignDto, Campaign>();
        CreateMap<PagedResult<Campaign>, PagedResult<GetCampaignDto>>();
    }
}
