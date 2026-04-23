using AutoMapper;
using ETicaretAPI.Common.Application.Interfaces;
using ETicaretAPI.Common.Application.Services;
using ETicaretAPI.Services.Catalog.Application.Repositories;
using ETicaretAPI.Services.Catalog.Application.Services.BrandService;
using ETicaretAPI.Services.Catalog.Application.Services.CategoryService;
using ETicaretAPI.Services.Catalog.Application.Services.ProductService;
using ETicaretAPI.Services.Catalog.Application.Services.ProductImageService;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using ETicaretAPI.Common.Infrastructure.ApiService;
using ETicaretAPI.Common.Infrastructure.ApiService.Rest.Microsoft;

namespace ETicaretAPI.Services.Catalog.Application;

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
        services.AddScoped<ICategoryService, CategoryManager>();
        services.AddScoped<IBrandService, BrandManager>();
        services.AddScoped<IProductService, ProductManager>();
        services.AddScoped<IProductImageService, ProductImageManager>();
        services.AddScoped<IRestApiService, MicrosoftRestApiService>();
        return services;
    }
}
