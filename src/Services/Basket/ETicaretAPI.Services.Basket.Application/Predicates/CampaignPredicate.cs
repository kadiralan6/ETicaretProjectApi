using System.Linq.Expressions;
using ETicaretAPI.Common.Infrastructure.Extensions;
using ETicaretAPI.Services.Basket.Domain.DTOs.CampaignDtos;
using ETicaretAPI.Services.Basket.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Application.Predicates;

public static class CampaignPredicate
{
    public static Expression<Func<Campaign, bool>> GetExpression(GetCampaignForAdminFilterDto filterDto)
    {
        var predicate = PredicateBuilder.True<Campaign>();

        if (!string.IsNullOrWhiteSpace(filterDto.Name))
            predicate = predicate.And(c => c.Name != null && c.Name.Contains(filterDto.Name));

        if (filterDto.Type.HasValue)
            predicate = predicate.And(c => c.Type == filterDto.Type.Value);

        if (filterDto.IsActive.HasValue)
            predicate = predicate.And(c => c.IsActive == filterDto.IsActive.Value);

        if (filterDto.ActiveOn.HasValue)
        {
            var date = filterDto.ActiveOn.Value;
            predicate = predicate.And(c => c.StartDate <= date && c.EndDate >= date);
        }

        return predicate;
    }
}
