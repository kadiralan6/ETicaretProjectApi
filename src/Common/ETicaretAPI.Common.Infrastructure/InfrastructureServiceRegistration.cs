using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace ETicaretAPI.Common.Infrastructure;

/// <summary>
/// Infrastructure servislerinin DI kaydını yapar.
/// Her mikroservisin Startup'ında çağrılır.
/// </summary>
public static class InfrastructureServiceRegistration
{
  public static IServiceCollection AddCommonInfrastructure(this IServiceCollection services, IConfiguration configuration)
  {
    // Redis Cache
    services.AddStackExchangeRedisCache(options =>
    {
      options.Configuration = configuration["RedisConfiguration:ConnectionString"] ?? configuration.GetConnectionString("Redis");
      options.InstanceName = configuration["RedisConfiguration:InstanceName"] ?? "ETicaretAPI_";
    });

    // RabbitMQ
    services.AddSingleton<IConnection>(sp =>
    {
      var factory = new ConnectionFactory
      {
        HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
        Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
        UserName = configuration["RabbitMQ:UserName"] ?? "guest",
        Password = configuration["RabbitMQ:Password"] ?? "guest",
        DispatchConsumersAsync = true
      };
      return factory.CreateConnection();
    });

    // HttpClient for Rest Service
    services.AddHttpClient("ServiceClient", client =>
    {
      client.Timeout = TimeSpan.FromSeconds(30);
    });

    return services;
  }
}
