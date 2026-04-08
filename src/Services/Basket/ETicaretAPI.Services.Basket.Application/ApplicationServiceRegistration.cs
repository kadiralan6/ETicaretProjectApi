
using Microsoft.Extensions.DependencyInjection;
using ETicaretAPI.Common.Infrastructure.ApiService;
using ETicaretAPI.Common.Infrastructure.ApiService.Rest.Microsoft;

namespace ETicaretAPI.Services.Basket.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IRestApiService, MicrosoftRestApiService>();

        return services;
    }
}
