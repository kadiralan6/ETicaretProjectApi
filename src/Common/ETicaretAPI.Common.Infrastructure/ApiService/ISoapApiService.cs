namespace ETicaretAPI.Common.Infrastructure.ApiService;

public interface ISoapApiService
{
    Task<T> GetAsync<T>(string endpoint, object? queryParams = null);
    Task<T> PostAsync<T>(string endpoint, object body, string? methodName = null);
}