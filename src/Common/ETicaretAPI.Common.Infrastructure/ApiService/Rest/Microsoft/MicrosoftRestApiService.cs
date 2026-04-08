using System.Net.Http.Json;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Infrastructure.Helpers;


namespace ETicaretAPI.Common.Infrastructure.ApiService.Rest.Microsoft;

public class MicrosoftRestApiService : IRestApiService
{
    private readonly HttpClient _httpClient;

    public MicrosoftRestApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<T> GetAsync<T>(string endpoint, object? queryParams = null)
    {
        var url = endpoint;

        if (queryParams != null)
        {
            var queryString = QueryStringHelper.ToQueryString(queryParams);
            url += "?" + queryString;
        }

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<T>();
        return result ?? default(T)!;
    }


    public async Task<T> GetDataResultAsync<T>(string endpoint, object? queryParams = null)
    {
        try
        {
            var url = BuildUrlWithQueryParams(endpoint, queryParams);
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            return ProcessDataResult<T>(responseBody);
        }
        catch
        {
            return default(T)!;
        }
    }

    private static string BuildUrlWithQueryParams(string endpoint, object? queryParams)
    {
        var url = endpoint;
        if (queryParams != null)
        {
            var queryString = QueryStringHelper.ToQueryString(queryParams);
            url += "?" + queryString;
        }
        return url;
    }

    private static T ProcessDataResult<T>(string responseBody)
    {

        var jObject = JObject.Parse(responseBody);
        var data = jObject["data"];

        if (data == null || data.Type == JTokenType.Null)
        {
            return default(T)!;
        }

        // If data is an empty array, return an empty list or default value
        if (data.Type == JTokenType.Array && !data.HasValues)
        {
            return HandleEmptyArray<T>();
        }

        // String değeri kontrol et
        if (data.Type == JTokenType.String)
        {
            // Eğer T bir List ise ve data string ise, boş liste döndür
            if (IsListType<T>())
            {
                return (T)Activator.CreateInstance(typeof(T))!; // Boş liste
            }

            // Eğer T string ise, direkt döndür
            if (typeof(T) == typeof(string))
            {
                return (T)(object)data.Value<string>()!;
            }

            // Eğer T Guid ise
            if (typeof(T) == typeof(Guid) || typeof(T) == typeof(Guid?))
            {
                var guidValue = data.Value<string>();
                if (Guid.TryParse(guidValue, out Guid guid))
                {
                    return (T)(object)guid;
                }
            }

            return default(T)!;
        }

        // Handle boolean values
        if (data.Type == JTokenType.Boolean)
        {
            if (typeof(T) == typeof(bool))
            {
                return (T)(object)data.Value<bool>();
            }
        }

        var dataJson = data.ToString();

        // Handle direct arrays first (before parsing to JObject)
        if (data.Type == JTokenType.Array)
        {
            return HandleArrayData<T>(data, dataJson);
        }

        // Check if we're trying to deserialize to a List<T> and the data is an object with nested arrays
        if (IsListType<T>())
        {
            // If expecting a List but got a single object, wrap it in an array
            var wrappedJson = $"[{dataJson}]";
            var deserializedArray = JsonConvert.DeserializeObject<T>(wrappedJson);
            return deserializedArray ?? default(T)!;
        }

        var deserializedObject = JsonConvert.DeserializeObject<T>(dataJson);
        return deserializedObject ?? default(T)!;

    }

    private static T HandleEmptyArray<T>()
    {
        if (IsListType<T>())
        {
            return (T)Activator.CreateInstance(typeof(T))!;
        }
        return default(T)!;
    }

    private static T HandleArrayData<T>(JToken data, string dataJson)
    {
        if (IsListType<T>())
        {
            // It's already an array, just deserialize it directly
            var deserializedArray = JsonConvert.DeserializeObject<T>(dataJson);
            return deserializedArray ?? default(T)!;
        }
        else
        {
            // If data is an array but T is not a collection type, take the first element
            var array = (JArray)data;
            if (array.Count > 0)
            {
                dataJson = array[0].ToString();
                var deserializedObject = JsonConvert.DeserializeObject<T>(dataJson);
                return deserializedObject ?? default(T)!;
            }
            else
            {
                return default(T)!;
            }
        }
    }

    private static bool IsListType<T>()
    {
        return typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>);
    }

    public async Task<T> PostAsync<T>(string endpoint, object body, string? methodName = null)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, body);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<T>();
            return result ?? default(T)!;
        }
        catch
        {
            return default(T)!; // Handle exceptions as needed, possibly log them
        }
    }

    public async Task<T> PostDataResultAsync<T>(string endpoint, object body, string? methodName = null)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, body);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            var jObject = JObject.Parse(responseBody);
            var data = jObject["data"];

            if (data == null || data.Type == JTokenType.Null)
            {
                return default(T)!;
            }

            // If data is an empty array, return an empty list or default value
            if (data.Type == JTokenType.Array && !data.HasValues)
            {
                if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
                {
                    return (T)Activator.CreateInstance(typeof(T))!;
                }
                return default(T)!;
            }

            // Handle boolean values - ÖNCELİK VER
            if (data.Type == JTokenType.Boolean)
            {
                if (typeof(T) == typeof(bool) || typeof(T) == typeof(bool?))
                {
                    return (T)(object)data.Value<bool>();
                }
            }

            var dataJson = data.ToString();

            // Handle direct arrays first (before parsing to JObject)
            if (data.Type == JTokenType.Array)
            {
                if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
                {
                    // It's already an array, just deserialize it directly
                    var deserializedArray = JsonConvert.DeserializeObject<T>(dataJson);
                    return deserializedArray ?? default(T)!;
                }
                else
                {
                    // If data is an array but T is not a collection type, take the first element
                    var array = (JArray)data;
                    if (array.Count > 0)
                    {
                        dataJson = array[0].ToString();
                    }
                    else
                    {
                        return default(T)!;
                    }
                }
            }
            // Check if we're trying to deserialize to a List<T> and the data is an object with nested arrays
            else if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
            {
                var dataObject = JObject.Parse(dataJson);

                // Look for array properties in the data object
                var arrayProperty = dataObject.Properties().FirstOrDefault(p => p.Value.Type == JTokenType.Array);

                if (arrayProperty != null)
                {
                    // Use the array property value instead of the whole object
                    dataJson = arrayProperty.Value.ToString();
                }
            }

            var deserializedObject = JsonConvert.DeserializeObject<T>(dataJson);

            return deserializedObject ?? default(T)!;
        }
        catch
        {
            return default(T)!;
        }
    }

    public async Task<T> PutDataResultAsync<T>(string endpoint, object body, string? methodName = null)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(endpoint, body);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            var jObject = JObject.Parse(responseBody);
            var data = jObject["data"];

            if (data == null || data.Type == JTokenType.Null)
            {
                return default(T)!;
            }

            // If data is an empty array, return an empty list or default value
            if (data.Type == JTokenType.Array && !data.HasValues)
            {
                if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
                {
                    return (T)Activator.CreateInstance(typeof(T))!;
                }
                return default(T)!;
            }

            var dataJson = data.ToString();

            // Handle direct arrays first (before parsing to JObject)
            if (data.Type == JTokenType.Array)
            {
                if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
                {
                    // It's already an array, just deserialize it directly
                    var deserializedArray = JsonConvert.DeserializeObject<T>(dataJson);
                    return deserializedArray ?? default(T)!;
                }
                else
                {
                    // If data is an array but T is not a collection type, take the first element
                    var array = (JArray)data;
                    if (array.Count > 0)
                    {
                        dataJson = array[0].ToString();
                    }
                    else
                    {
                        return default(T)!;
                    }
                }
            }
            // Check if we're trying to deserialize to a List<T> and the data is an object with nested arrays
            else if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
            {
                var dataObject = JObject.Parse(dataJson);

                // Look for array properties in the data object
                foreach (var property in dataObject.Properties())
                {
                    if (property.Value is JArray)
                    {
                        // Deserialize the array property as the result
                        return JsonConvert.DeserializeObject<T>(property.Value.ToString()) ?? default(T)!;
                    }
                }
            }

            var deserializedObject = JsonConvert.DeserializeObject<T>(dataJson);

            return deserializedObject ?? default(T)!;
        }
        catch
        {
            return default(T)!;
        }
    }

    public async Task<T> PostFileDataResultAsync<T>(string endpoint, List<IFormFile> files, int usageValueId, int? userId = null)
    {
        try
        {
            using var content = new MultipartFormDataContent();

            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].Length > 0)
                {
                    var fileContent = new StreamContent(files[i].OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(files[i].ContentType);
                    // CreateMediaDto.Files property'si için doğru field name
                    content.Add(fileContent, "Files", files[i].FileName);
                }
            }

            content.Add(new StringContent(usageValueId.ToString()), "UsageValueId");

            if (userId.HasValue)
                content.Add(new StringContent(userId.Value.ToString()), "UserId");

            var response = await _httpClient.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(responseBody);
            var data = jObject["data"];

            if (data == null || data.Type == JTokenType.Null)
            {
                return default(T)!;
            }

            var dataJson = data.ToString();
            var deserializedObject = JsonConvert.DeserializeObject<T>(dataJson);

            return deserializedObject ?? default(T)!;
        }
        catch
        {
            return default(T)!;
        }
    }
    public async Task<T> PutFileDataResultAsync<T>(string endpoint, List<IFormFile> files, int usageValueId)
    {
        try
        {
            using var content = new MultipartFormDataContent();

            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].Length > 0)
                {
                    var fileContent = new StreamContent(files[i].OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(files[i].ContentType);
                    // DTO'daki isimle aynı olmalı: "NewFile"
                    content.Add(fileContent, "NewFile", files[i].FileName);
                }
            }

            content.Add(new StringContent(usageValueId.ToString()), "MediaId");

            var response = await _httpClient.PutAsync(endpoint, content);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(responseBody);
            var dataJson = jObject["data"]?.ToString();

            if (string.IsNullOrWhiteSpace(dataJson))
            {
                return default(T)!;
            }

            var deserializedObject = JsonConvert.DeserializeObject<T>(dataJson);

            return deserializedObject ?? default(T)!;
        }
        catch
        {
            return default(T)!;
        }
    }
    public async Task<T> PostFileResultAsync<T>(string endpoint, List<IFormFile> files, int usageValueId, Guid? userId = null)
    {
        try
        {
            using var content = new MultipartFormDataContent();

            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].Length > 0)
                {
                    var fileContent = new StreamContent(files[i].OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(files[i].ContentType);
                    content.Add(fileContent, "Files", files[i].FileName);
                }
            }

            content.Add(new StringContent(usageValueId.ToString()), "UsageValueId");
            content.Add(new StringContent(userId?.ToString() ?? ""), "UserId");

            var response = await _httpClient.PostAsync(endpoint, content);

            string responseBody = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseBody);
                var message = apiResponse?.Message
                    ?? apiResponse?.Errors?.FirstOrDefault()
                    ?? "Media API istegi basarisiz oldu.";
                throw new InvalidOperationException(message);
            }

            var jObject = JObject.Parse(responseBody);
            var data = jObject["data"];
            if (data == null || data.Type == JTokenType.Null)
            {
                return default(T)!;
            }

            var dataJson = data.ToString();
            var deserializedObject = JsonConvert.DeserializeObject<T>(dataJson);
            return deserializedObject ?? default(T)!;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Media API istegi basarisiz oldu: {ex.Message}", ex);
        }

    }

    public async Task<T> PutFileResultAsync<T>(string endpoint, List<IFormFile> files, int usageValueId)
    {
        try
        {
            using var content = new MultipartFormDataContent();

            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].Length > 0)
                {
                    var fileContent = new StreamContent(files[i].OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(files[i].ContentType);
                    // DTO'daki isimle aynı olmalı: "NewFile"
                    content.Add(fileContent, "NewFile", files[i].FileName);
                }
            }

            content.Add(new StringContent(usageValueId.ToString()), "UsageValueId");
            content.Add(new StringContent(usageValueId.ToString()), "MediaId");

            var response = await _httpClient.PutAsync(endpoint, content);

            string responseBody = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseBody);
                var message = apiResponse?.Message
                    ?? apiResponse?.Errors?.FirstOrDefault()
                    ?? "Media API istegi basarisiz oldu.";
                throw new InvalidOperationException(message);
            }

            var deserializedObject = JsonConvert.DeserializeObject<T>(responseBody);

            return deserializedObject ?? default(T)!;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Media API istegi basarisiz oldu: {ex.Message}", ex);
        }

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
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseBody);
                var message = apiResponse?.Message
                    ?? apiResponse?.Errors?.FirstOrDefault()
                    ?? "Media API istegi basarisiz oldu.";
                throw new InvalidOperationException(message);
            }

            var deserializedObject = JsonConvert.DeserializeObject<T>(responseBody);

            return deserializedObject ?? default(T)!;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Media API istegi basarisiz oldu: {ex.Message}", ex);
        }
    }
}