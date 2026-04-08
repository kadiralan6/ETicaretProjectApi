using ETicaretAPI.Common.Persistence.DataAccess.EntityFramework;
using ETicaretAPI.Services.Identity.Domain.Entities;
using ETicaretAPI.Services.Identity.Domain.Interfaces.Repositories;
using ETicaretAPI.Services.Identity.Persistence.Context;

namespace ETicaretAPI.Services.Identity.Persistence.Repositories;

public class UserRepository : EfEntityRepositoryBase<User, IdentityDbContext>, IUserRepository
{
    public UserRepository(IdentityDbContext context) : base(context)
    {
    }
}
