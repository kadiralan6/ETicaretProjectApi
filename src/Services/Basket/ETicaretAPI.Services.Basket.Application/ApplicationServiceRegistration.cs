using System.Reflection;
using ETicaretAPI.Common.Application.Interfaces;
using ETicaretAPI.Common.Application.Services;
using ETicaretAPI.Common.Infrastructure.ApiService;
using ETicaretAPI.Common.Infrastructure.ApiService.Rest.Microsoft;
using ETicaretAPI.Common.Infrastructure.Mappings;
using ETicaretAPI.Services.Basket.Application.Services.CampaignService;
using ETicaretAPI.Services.Basket.Application.Services.CartService;
using ETicaretAPI.Services.Basket.Application.Services.CouponService;
using Microsoft.Extensions.DependencyInjection;

namespace ETicaretAPI.Services.Basket.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // AutoMapper — tüm Profile'ları bu assembly'den tarar
        services.AddAutoMapper(cfg =>
        {
            cfg.AddMaps(Assembly.GetExecutingAssembly());
            cfg.AddProfile<GlobalMappingProfile>();
        });

        // Services
        services.AddScoped<ICartService, CartManager>();
        services.AddScoped<ICouponService, CouponManager>();
        services.AddScoped<ICampaignService, CampaignManager>();

        services.AddSingleton<ICacheService, RedisCacheManager>();
        services.AddSingleton<IEventBus, RabbitMQEventBusManager>();

        services.AddScoped<IRestApiService, MicrosoftRestApiService>();

        return services;
    }
}
