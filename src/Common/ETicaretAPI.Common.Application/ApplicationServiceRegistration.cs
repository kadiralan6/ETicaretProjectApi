using ETicaretAPI.Common.Application.Interfaces;
using ETicaretAPI.Common.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ETicaretAPI.Common.Application;

public static class ApplicationServiceRegistration
{
  public static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    services.AddSingleton<ICacheService, RedisCacheManager>();
    services.AddSingleton<IEventBus, RabbitMQEventBusManager>();
    services.AddScoped<IRestService, RestManager>();

    return services;
  }
}
