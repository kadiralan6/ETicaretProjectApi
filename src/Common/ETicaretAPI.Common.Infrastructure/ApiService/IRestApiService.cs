using Microsoft.AspNetCore.Http;

namespace ETicaretAPI.Common.Infrastructure.ApiService;

public interface IRestApiService
{
    Task<T> GetAsync<T>(string endpoint, object? queryParams = null);
    Task<T> PostAsync<T>(string endpoint, object body, string? methodName = null);
    Task<T> PostDataResultAsync<T>(string endpoint, object body, string? methodName = null);
    Task<T> PutDataResultAsync<T>(string endpoint, object body, string? methodName = null);
    Task<T> GetDataResultAsync<T>(string endpoint, object? queryParams = null);
    Task<T> PostFileDataResultAsync<T>(string endpoint, List<IFormFile> files, int usageValueId, int? userId = null);
    Task<T> PutFileDataResultAsync<T>(string endpoint, List<IFormFile> files, int usageValueId);

    Task<T> PostFileResultAsync<T>(string endpoint, List<IFormFile> files, int usageValueId, Guid? userId = null);
    Task<T> PutFileResultAsync<T>(string endpoint, List<IFormFile> files, int usageValueId);
    Task<T> PostMultipartFormDataAsync<T>(string endpoint, List<IFormFile> files);

}