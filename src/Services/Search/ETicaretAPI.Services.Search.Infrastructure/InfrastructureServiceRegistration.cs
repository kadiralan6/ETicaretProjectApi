using ETicaretAPI.Services.Search.Application.Repositories;
using ETicaretAPI.Services.Search.Infrastructure.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace ETicaretAPI.Services.Search.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddSearchInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        // Elasticsearch client
        var elasticsearchUrl = configuration["Elasticsearch:Url"] ?? "http://localhost:9200";

        var settings = new ConnectionSettings(new Uri(elasticsearchUrl))
            .DefaultIndex("products")
            .EnableDebugMode()
            .PrettyJson()
            .RequestTimeout(TimeSpan.FromSeconds(30))
            .MaxRetryTimeout(TimeSpan.FromSeconds(60))
            .ThrowExceptions(false);

        var client = new ElasticClient(settings);
        services.AddSingleton<IElasticClient>(client);

        // Repository
        services.AddScoped<IProductSearchRepository, ElasticsearchProductRepository>();

        // Index initializer (runs on startup)
        services.AddHostedService<ElasticsearchIndexInitializer>();

        return services;
    }
}
