using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Common.Persistence.Repositories;
using ETicaretAPI.Services.Payment.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ETicaretAPI.Services.Payment.Infrastructure;

public static class PaymentServiceRegistration
{
  public static IServiceCollection AddPaymentServices(this IServiceCollection services, IConfiguration configuration)
  {
    // DbContext
    services.AddDbContext<PaymentDbContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

    // Repositories
    services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
    services.AddScoped<IUnitOfWork>(sp =>
        new UnitOfWork(sp.GetRequiredService<PaymentDbContext>()));

    return services;
  }
}
