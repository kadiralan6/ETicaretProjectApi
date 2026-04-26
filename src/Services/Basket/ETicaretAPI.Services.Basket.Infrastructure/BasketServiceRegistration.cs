using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ETicaretAPI.Services.Basket.Infrastructure;

/// <summary>
/// Basket servisine özgü üçüncü taraf entegrasyon servisleri buraya eklenir
/// (ör. ödeme sağlayıcısı, bildirim servisi, vb.).
/// DbContext ve repository kayıtları için <c>PersistenceServiceRegistration</c> kullanın.
/// </summary>
public static class BasketServiceRegistration
{
    public static IServiceCollection AddBasketInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}
