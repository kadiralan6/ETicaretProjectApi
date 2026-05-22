using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Services.Identity.Domain.Entities;

namespace ETicaretAPI.Services.Identity.Domain.Interfaces.Repositories;

public interface IAddressRepository : IEntityRepository<Address>
{
    Task<IEnumerable<Address>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<Address?> GetDefaultByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}
