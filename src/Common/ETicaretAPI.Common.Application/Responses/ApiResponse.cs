namespace ETicaretAPI.Common.Application.Responses;


public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public string? ErrorCode { get; set; }
    public List<string> Errors { get; set; } = new();
    public int StatusCode { get; set; }

    public static ApiResponse<T> Success(T data, string? message = null, int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            Data = data,
            Message = message,
            StatusCode = statusCode
        };
    }

    public static ApiResponse<T> Fail(string error, int statusCode = 400, string? errorCode = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            Errors = new List<string> { error },
            ErrorCode = errorCode,
            StatusCode = statusCode
        };
    }

    public static ApiResponse<T> Fail(List<string> errors, int statusCode = 400, string? errorCode = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            Errors = errors,
            ErrorCode = errorCode,
            StatusCode = statusCode
        };
    }
}


public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse Success(string? message = null, int statusCode = 200)
    {
        return new ApiResponse
        {
            IsSuccess = true,
            Message = message,
            StatusCode = statusCode
        };
    }

    public new static ApiResponse Fail(string error, int statusCode = 400, string? errorCode = null)
    {
        return new ApiResponse
        {
            IsSuccess = false,
            Errors = new List<string> { error },
            ErrorCode = errorCode,
            StatusCode = statusCode
        };
    }

    public new static ApiResponse Fail(List<string> errors, int statusCode = 400, string? errorCode = null)
    {
        return new ApiResponse
        {
            IsSuccess = false,
            Errors = errors,
            ErrorCode = errorCode,
            StatusCode = statusCode
        };
    }

    public static ApiResponse Error(string error, int statusCode = 400)
    {
        return Fail(error, statusCode);
    }
}
