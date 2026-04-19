using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Common.Persistence.Repositories;
using ETicaretAPI.Services.Catalog.Application.Repositories;
using ETicaretAPI.Services.Catalog.Persistence.Context;
using ETicaretAPI.Services.Catalog.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ETicaretAPI.Services.Catalog.Application.Services.ImageUploadService;
using ETicaretAPI.Services.Catalog.Infrastructure.Services.ImageUploadService;

namespace ETicaretAPI.Services.Catalog.Infrastructure;

public static class CatalogServiceRegistration
{
    public static IServiceCollection AddCatalogServices(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Rep
        services.AddScoped<IUnitOfWork>(sp =>
            new UnitOfWork(sp.GetRequiredService<CatalogDbContext>()));

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IImageUploadService, CloudinaryImageUploadService>();

        return services;
    }
}
