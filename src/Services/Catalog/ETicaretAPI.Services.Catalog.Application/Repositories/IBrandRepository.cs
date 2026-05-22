using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Services.Catalog.Domain.Entities;

namespace ETicaretAPI.Services.Catalog.Application.Repositories;

public interface IBrandRepository : IEntityRepository<Brand>
{
    /// <summary>
    /// Belirli bir slug'ın başka bir marka tarafından kullanılıp kullanılmadığını kontrol eder.
    /// </summary>
    Task<bool> IsSlugExistsAsync(string slug, int? excludeId = null, CancellationToken cancellationToken = default);
}
