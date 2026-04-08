using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Infrastructure.Configuration;
using ETicaretAPI.Common.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;


namespace ETicaretAPI.Common.Infrastructure.ApiService.Rest.RestSharp;

// Birden fazla url kullanacağımız ve RestSharp ile RestClient oluştururken baseUrl'i 
// aldığı için şimdilik MicrosoftRestApiService kullanıyoruz.
public class RestSharpRestApiService : IRestApiService
{
    private RestClient _client;
    private readonly HttpClient _httpClient;
    private readonly List<string> _baseUrls;


    public RestSharpRestApiService()
    {
        _baseUrls = CoreConfig.GetSectionValue<List<string>>("ExternalApi:BaseUrls");
        var baseUrl = _baseUrls.FirstOrDefault();
        _client = new RestClient(baseUrl);
        _httpClient = new HttpClient();
    }

    public async Task<T> GetAsync<T>(string endpoint, object? queryParams = null)
    {
        var request = new RestRequest(endpoint, Method.Get);

        if (queryParams != null)
        {
            var queryString = QueryStringHelper.ToQueryString(queryParams);
            request.Resource += "?" + queryString;
        }

        var response = await _client.ExecuteAsync<T>(request);
        return JsonConvert.DeserializeObject<T>(response.Content);
    }

    public async Task<T> GetDataResultAsync<T>(string endpoint, object? queryParams = null)
    {
        var request = new RestRequest(endpoint, Method.Get);

        if (queryParams != null)
        {
            var queryString = QueryStringHelper.ToQueryString(queryParams);
            request.Resource += "?" + queryString;
        }

        var response = await _client.ExecuteAsync<T>(request);

        var jObject = JObject.Parse(response.Content);
        var dataJson = jObject["data"].ToString();

        var deserializedObject = JsonConvert.DeserializeObject<T>(dataJson);

        return deserializedObject;
    }

    public async Task<T> PostAsync<T>(string endpoint, object body, string? methodName = null)
    {
        var request = new RestRequest(endpoint, Method.Post);
        request.AddJsonBody(body);
        var response = await _client.ExecuteAsync<T>(request);
        return response.Data;
    }

    public async Task<T> PostDataResultAsync<T>(string endpoint, object body, string? methodName = null)
    {
        if (endpoint != _client.Options.BaseUrl.OriginalString)
        {
            var baseUrl = _baseUrls.FirstOrDefault(x => endpoint.StartsWith(x, StringComparison.OrdinalIgnoreCase));
            _client = new RestClient(baseUrl);
        }

        var request = new RestRequest(endpoint, Method.Post);
        request.AddJsonBody(body);
        var response = await _client.ExecuteAsync<T>(request);

        var jObject = JObject.Parse(response.Content);
        var dataJson = jObject["data"].ToString();

        var deserializedObject = JsonConvert.DeserializeObject<T>(dataJson);

        return deserializedObject;
    }

    public async Task<T> PutDataResultAsync<T>(string endpoint, object body, string? methodName = null)
    {
        if (endpoint != _client.Options.BaseUrl.OriginalString)
        {
            var baseUrl = _baseUrls.FirstOrDefault(x => endpoint.StartsWith(x, StringComparison.OrdinalIgnoreCase));
            _client = new RestClient(baseUrl);
        }

        var request = new RestRequest(endpoint, Method.Put);
        request.AddJsonBody(body);
        var response = await _client.ExecuteAsync<T>(request);

        var jObject = JObject.Parse(response.Content);
        var dataJson = jObject["data"].ToString();

        var deserializedObject = JsonConvert.DeserializeObject<T>(dataJson);

        return deserializedObject;
    }

    // TODO: ParamKeyValue alınabilir.
    public async Task<T> PostFileDataResultAsync<T>(string endpoint, List<IFormFile> files, int usageValueId, int? userId = null)
    {
        var request = new RestRequest(endpoint, Method.Post);
        request.AddHeader("Content-Type", "multipart/form-data");

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                request.AddFile("files", () => file.OpenReadStream(), file.FileName, file.ContentType);
            }
        }

        request.AddParameter("usageValueId", usageValueId.ToString());

        // Add userId parameter if provided
        if (userId.HasValue)
        {
            request.AddParameter("userId", userId.Value.ToString());
        }

        var response = await _client.ExecuteAsync<T>(request);

        if (string.IsNullOrEmpty(response.Content))
            return default(T)!;

        var jObject = JObject.Parse(response.Content);
        var dataJson = jObject["data"]?.ToString();

        if (string.IsNullOrEmpty(dataJson))
            return default(T)!;

        var deserializedObject = JsonConvert.DeserializeObject<T>(dataJson);

        return deserializedObject ?? default(T)!;
    }

    public async Task<T> PutFileDataResultAsync<T>(string endpoint, List<IFormFile> files, int usageValueId)
    {
        var request = new RestRequest(endpoint, Method.Put);
        request.AlwaysMultipartFormData = true;

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                request.AddFile("files", () => file.OpenReadStream(), file.FileName, file.ContentType);
            }
        }

        request.AddParameter("usageValueId", usageValueId.ToString());

        var response = await _client.ExecuteAsync(request);

        if (!response.IsSuccessful)
            throw new Exception($"PUT failed: {response.StatusCode} - {response.Content}");

        if (string.IsNullOrEmpty(response.Content))
            return default(T)!;

        var jObject = JObject.Parse(response.Content);
        var dataJson = jObject["data"]?.ToString();

        if (string.IsNullOrEmpty(dataJson))
            return default(T)!;

        var deserializedObject = JsonConvert.DeserializeObject<T>(dataJson);

        return deserializedObject ?? default(T)!;
    }

    public async Task<T> PostFileResultAsync<T>(string endpoint, List<IFormFile> files, int usageValueId, Guid? userId = null)
    {
        var request = new RestRequest(endpoint, Method.Post);
        request.AddHeader("Content-Type", "multipart/form-data");

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                request.AddFile("files", () => file.OpenReadStream(), file.FileName, file.ContentType);
            }
        }

        request.AddParameter("usageValueId", usageValueId.ToString());

        if (userId.HasValue)
        {
            request.AddParameter("userId", userId.Value.ToString());
        }

        var response = await _client.ExecuteAsync<T>(request);

        if (string.IsNullOrEmpty(response.Content))
            return default(T)!;


        var deserializedObject = JsonConvert.DeserializeObject<T>(response.Content);

        return deserializedObject ?? default(T)!;
    }

    public async Task<T> PutFileResultAsync<T>(string endpoint, List<IFormFile> files, int usageValueId)
    {
        var request = new RestRequest(endpoint, Method.Put);
        request.AlwaysMultipartFormData = true;

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                request.AddFile("files", () => file.OpenReadStream(), file.FileName, file.ContentType);
            }
        }

        request.AddParameter("usageValueId", usageValueId.ToString());

        var response = await _client.ExecuteAsync(request);

        if (!response.IsSuccessful)
            throw new Exception($"PUT failed: {response.StatusCode} - {response.Content}");

        if (string.IsNullOrEmpty(response.Content))
            return default(T)!;


        var deserializedObject = JsonConvert.DeserializeObject<T>(response.Content);

        return deserializedObject ?? default(T)!;
    }

    public async Task<T> PostMultipartFormDataAsync<T>(string endpoint, List<IFormFile> files)
    {
        try
        {
            using var content = new MultipartFormDataContent();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    // Stream'i MemoryStream'e kopyala
                    var memoryStream = new MemoryStream();
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    var fileContent = new StreamContent(memoryStream);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                    content.Add(fileContent, "files", file.FileName);
                }
            }

            var response = await _httpClient.PostAsync(endpoint, content);

            string responseBody = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                var errorDataResult = JsonConvert.DeserializeObject<ApiResponse>(responseBody);
                throw new Exception($"API Error: {errorDataResult?.Message ?? "Unknown error"}");
            }

            var deserializedObject = JsonConvert.DeserializeObject<T>(responseBody);

            return deserializedObject ?? default(T)!;
        }
        catch (Exception bex)
        {
            throw;
        }
    }

}