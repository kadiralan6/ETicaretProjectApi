using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Services.Identity.Domain.Entities;

namespace ETicaretAPI.Services.Identity.Domain.Interfaces.Repositories;

public interface IUserRepository : IEntityRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> IsEmailExistsAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default);
}
