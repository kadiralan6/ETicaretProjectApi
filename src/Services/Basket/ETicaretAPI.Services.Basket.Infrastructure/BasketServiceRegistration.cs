using ETicaretAPI.Services.Basket.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ETicaretAPI.Services.Basket.Infrastructure;

public static class BasketServiceRegistration
{
  public static IServiceCollection AddBasketServices(this IServiceCollection services, IConfiguration configuration)
  {
    // DbContext (kampanya ve kupon verisi için)
    services.AddDbContext<BasketDbContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

    return services;
  }
}
