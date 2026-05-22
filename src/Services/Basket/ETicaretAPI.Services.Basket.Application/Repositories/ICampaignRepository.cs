using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Services.Basket.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Application.Repositories;

public interface ICampaignRepository : IEntityRepository<Campaign>
{
    /// <summary>
    /// Belirli bir tarihte aktif olan kampanyaları getirir.
    /// </summary>
    Task<List<Campaign>> GetActiveCampaignsOnDateAsync(DateTime date, CancellationToken cancellationToken = default);
}
