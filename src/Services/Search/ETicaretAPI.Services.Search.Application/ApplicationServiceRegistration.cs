using ETicaretAPI.Common.Application.Interfaces;
using ETicaretAPI.Common.SharedLibrary.Events;
using ETicaretAPI.Services.Search.Application.EventHandlers;
using ETicaretAPI.Services.Search.Application.Services.SearchService;
using ETicaretAPI.Common.Infrastructure.ApiService;
using ETicaretAPI.Common.Infrastructure.ApiService.Rest.Microsoft;
using Microsoft.Extensions.DependencyInjection;

namespace ETicaretAPI.Services.Search.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Services
        services.AddScoped<ISearchService, SearchManager>();

        // Event handlers
        services.AddScoped<ProductCreatedEventHandler>();
        services.AddScoped<ProductUpdatedEventHandler>();
        services.AddScoped<ProductDeletedEventHandler>();
        services.AddScoped<IRestApiService, MicrosoftRestApiService>();
        return services;
    }

}
