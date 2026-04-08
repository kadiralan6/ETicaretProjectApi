using ETicaretAPI.Common.Persistence.DataAccess.EntityFramework;
using ETicaretAPI.Services.Identity.Domain.Entities;
using ETicaretAPI.Services.Identity.Domain.Interfaces.Repositories;
using ETicaretAPI.Services.Identity.Persistence.Context;

namespace ETicaretAPI.Services.Identity.Persistence.Repositories;

public class AddressRepository : EfEntityRepositoryBase<Address, IdentityDbContext>, IAddressRepository
{
    public AddressRepository(IdentityDbContext context) : base(context)
    {
    }
}
