using ETicaretAPI.Services.Catalog.Application.Services.ImageUploadService;
using ETicaretAPI.Services.Catalog.Infrastructure.Services.ImageUploadService;
using Microsoft.Extensions.DependencyInjection;

namespace ETicaretAPI.Services.Catalog.Infrastructure;

public static class CatalogServiceRegistration
{
    public static IServiceCollection AddCatalogServices(this IServiceCollection services)
    {
        services.AddScoped<IImageUploadService, CloudinaryImageUploadService>();

        return services;
    }
}
