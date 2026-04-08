using AutoMapper;
using ETicaretAPI.Services.Catalog.Application.Repositories;
using ETicaretAPI.Services.Catalog.Application.Services.BrandService;
using ETicaretAPI.Services.Catalog.Application.Services.CategoryService;
using ETicaretAPI.Services.Catalog.Application.Services.ProductService;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

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

    // Services
    services.AddScoped<ICategoryService, CategoryManager>();
    services.AddScoped<IBrandService, BrandManager>();
    services.AddScoped<IProductService, ProductManager>();

    return services;
  }
}
