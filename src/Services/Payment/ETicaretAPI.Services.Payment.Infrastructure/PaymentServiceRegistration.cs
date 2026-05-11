using Microsoft.Extensions.DependencyInjection;

namespace ETicaretAPI.Services.Payment.Infrastructure;

public static class PaymentServiceRegistration
{
    public static IServiceCollection AddPaymentServices(this IServiceCollection services)
    {
        // Reserved for external 3rd-party integrations (e.g., payment gateway clients)
        return services;
    }
}
