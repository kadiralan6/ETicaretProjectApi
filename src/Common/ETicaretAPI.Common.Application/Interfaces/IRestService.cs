namespace ETicaretAPI.Common.Application.Interfaces;

/// <summary>
/// Servisler arası HTTP iletişimi için Rest Service interface'i.
/// </summary>
public interface IRestService
{
  Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default);
  Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data, CancellationToken cancellationToken = default);
  Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest data, CancellationToken cancellationToken = default);
  Task<bool> DeleteAsync(string url, CancellationToken cancellationToken = default);
}
