using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Common.Persistence.Repositories;
using ETicaretAPI.Services.Payment.Application.Repositories;
using ETicaretAPI.Services.Payment.Persistence.Context;
using ETicaretAPI.Services.Payment.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ETicaretAPI.Services.Payment.Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PaymentDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        }, ServiceLifetime.Scoped);

        services.AddScoped<DbContext>(sp => sp.GetRequiredService<PaymentDbContext>());

        services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
