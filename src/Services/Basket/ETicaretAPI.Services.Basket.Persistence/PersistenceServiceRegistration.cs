using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Common.Persistence.Repositories;
using ETicaretAPI.Services.Basket.Application.Repositories;
using ETicaretAPI.Services.Basket.Persistence.Context;
using ETicaretAPI.Services.Basket.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ETicaretAPI.Services.Basket.Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BasketDbContext>((_, options) =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")),
            ServiceLifetime.Scoped);

        services.AddScoped<DbContext>(sp => sp.GetRequiredService<BasketDbContext>());

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ICartItemsRepository, CartItemsRepository>();
        services.AddScoped<ICouponRepository, CouponRepository>();
        services.AddScoped<ICampaignRepository, CampaignRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();

        return services;
    }
}
