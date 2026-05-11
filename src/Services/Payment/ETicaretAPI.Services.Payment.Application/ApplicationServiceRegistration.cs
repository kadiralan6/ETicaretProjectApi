using System.Reflection;
using ETicaretAPI.Common.Application.Interfaces;
using ETicaretAPI.Common.Application.Services;
using ETicaretAPI.Services.Payment.Application.Services.PaymentTransactionService;
using Microsoft.Extensions.DependencyInjection;

namespace ETicaretAPI.Services.Payment.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(cfg =>
        {
            cfg.AddMaps(Assembly.GetExecutingAssembly());
            cfg.AddProfile<ETicaretAPI.Common.Infrastructure.Mappings.GlobalMappingProfile>();
        });

        // Cache
        services.AddSingleton<ICacheService, RedisCacheManager>();

        // Event Bus
        services.AddSingleton<IEventBus, RabbitMQEventBusManager>();

        // Services
        services.AddScoped<IPaymentTransactionService, PaymentTransactionManager>();

        return services;
    }
}
