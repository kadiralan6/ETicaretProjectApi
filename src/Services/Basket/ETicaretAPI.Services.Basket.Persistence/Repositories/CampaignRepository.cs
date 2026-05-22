using ETicaretAPI.Common.Persistence.DataAccess.EntityFramework;
using ETicaretAPI.Services.Basket.Application.Repositories;
using ETicaretAPI.Services.Basket.Domain.Entities;
using ETicaretAPI.Services.Basket.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Services.Basket.Persistence.Repositories;

public class CampaignRepository : EfEntityRepositoryBase<Campaign, BasketDbContext>, ICampaignRepository
{
    public CampaignRepository(BasketDbContext context) : base(context)
    {
    }

    public async Task<List<Campaign>> GetActiveCampaignsOnDateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        return await _context.Campaigns
            .AsNoTracking()
            .Where(c => c.IsActive && !c.IsDeleted && c.StartDate <= date && c.EndDate >= date)
            .OrderByDescending(c => c.DiscountValue)
            .ToListAsync(cancellationToken);
    }
}
