using ETicaretAPI.Services.Payment.Application.Interfaces;
using ETicaretAPI.Services.Payment.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ETicaretAPI.Services.Payment.Application;

public static class ApplicationServiceRegistration
{
  public static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    services.AddScoped<IOrderService, OrderManager>();
    services.AddScoped<IPaymentService, PaymentManager>();

    return services;
  }
}
