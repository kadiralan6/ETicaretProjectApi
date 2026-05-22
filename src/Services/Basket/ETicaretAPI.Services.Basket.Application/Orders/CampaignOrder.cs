using System.Linq.Expressions;
using ETicaretAPI.Common.Domain.Enums;
using ETicaretAPI.Services.Basket.Domain.Entities;
using ETicaretAPI.Services.Basket.Domain.Enums;

namespace ETicaretAPI.Services.Basket.Application.Orders;

public static class CampaignOrder
{
    public static List<(Expression<Func<Campaign, object>> orderSelector, bool orderAsc)> GetOrder(
        CampaignOrderByEnum orderBy, OrderTypeEnum orderType)
    {
        var orders = new List<(Expression<Func<Campaign, object>> orderSelector, bool orderAsc)>();

        Expression<Func<Campaign, object>> orderSelector;
        Expression<Func<Campaign, object>> createdAtSelector = x => x.CreatedAt;

        switch (orderBy)
        {
            case CampaignOrderByEnum.ModifiedAt:
                orderSelector = x => x.ModifiedAt!;
                break;
            case CampaignOrderByEnum.StartDate:
                orderSelector = x => x.StartDate;
                break;
            case CampaignOrderByEnum.EndDate:
                orderSelector = x => x.EndDate;
                break;
            case CampaignOrderByEnum.Name:
                orderSelector = x => x.Name!;
                break;
            default:
                orderSelector = x => x.CreatedAt;
                break;
        }

        orders.Add((orderSelector, orderType == OrderTypeEnum.ASC));

        if (orderBy != CampaignOrderByEnum.CreatedAt)
            orders.Add((createdAtSelector, orderType == OrderTypeEnum.ASC));

        return orders;
    }
}
