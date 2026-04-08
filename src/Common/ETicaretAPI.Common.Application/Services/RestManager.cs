using System.Net.Http.Json;
using ETicaretAPI.Common.Application.Interfaces;

namespace ETicaretAPI.Common.Application.Services;

/// <summary>
/// Servisler arası HTTP iletişimi için Rest Manager implementasyonu.
/// HttpClientFactory pattern kullanır.
/// </summary>
public class RestManager : IRestService
{
  private readonly IHttpClientFactory _httpClientFactory;

  public RestManager(IHttpClientFactory httpClientFactory)
  {
    _httpClientFactory = httpClientFactory;
  }

  public async Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default)
  {
    var client = _httpClientFactory.CreateClient("ServiceClient");
    var response = await client.GetAsync(url, cancellationToken);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
  }

  public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data, CancellationToken cancellationToken = default)
  {
    var client = _httpClientFactory.CreateClient("ServiceClient");
    var response = await client.PostAsJsonAsync(url, data, cancellationToken);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);
  }

  public async Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest data, CancellationToken cancellationToken = default)
  {
    var client = _httpClientFactory.CreateClient("ServiceClient");
    var response = await client.PutAsJsonAsync(url, data, cancellationToken);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);
  }

  public async Task<bool> DeleteAsync(string url, CancellationToken cancellationToken = default)
  {
    var client = _httpClientFactory.CreateClient("ServiceClient");
    var response = await client.DeleteAsync(url, cancellationToken);
    return response.IsSuccessStatusCode;
  }
}
