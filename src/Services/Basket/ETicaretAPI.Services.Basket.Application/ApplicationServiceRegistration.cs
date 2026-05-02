using System.Reflection;
using ETicaretAPI.Common.Application.Interfaces;
using ETicaretAPI.Common.Application.Services;
using ETicaretAPI.Common.Infrastructure.ApiService;
using ETicaretAPI.Common.Infrastructure.ApiService.Rest.Microsoft;
using ETicaretAPI.Common.Infrastructure.Mappings;
using ETicaretAPI.Services.Basket.Application.Services.CampaignService;
using ETicaretAPI.Services.Basket.Application.Services.CartItemsService;
using ETicaretAPI.Services.Basket.Application.Services.CouponService;
using ETicaretAPI.Services.Basket.Application.Services.OrderItemService;
using ETicaretAPI.Services.Basket.Application.Services.OrderService;
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
        services.AddScoped<ICartItemsService, CartItemsManager>();
        services.AddScoped<ICouponService, CouponManager>();
        services.AddScoped<ICampaignService, CampaignManager>();
        services.AddScoped<IOrderService, OrderManager>();
        services.AddScoped<IOrderItemService, OrderItemManager>();

        services.AddSingleton<ICacheService, RedisCacheManager>();
        services.AddSingleton<IEventBus, RabbitMQEventBusManager>();

        services.AddScoped<IRestApiService, MicrosoftRestApiService>();

        return services;
    }
}
