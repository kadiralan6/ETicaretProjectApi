using ETicaretAPI.Common.Persistence.DataAccess.EntityFramework;
using ETicaretAPI.Services.Identity.Domain.Entities;
using ETicaretAPI.Services.Identity.Domain.Interfaces.Repositories;
using ETicaretAPI.Services.Identity.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Services.Identity.Persistence.Repositories;

public class UserRepository : EfEntityRepositoryBase<User, IdentityDbContext>, IUserRepository
{
    public UserRepository(IdentityDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.UsersTable
            .AsNoTracking()
            .Where(u => u.Email == email && !u.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> IsEmailExistsAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.UsersTable
            .AsNoTracking()
            .Where(u => u.Email == email && !u.IsDeleted);

        if (excludeId.HasValue)
            query = query.Where(u => u.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }
}
