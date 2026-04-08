using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ETicaretAPI.Common.Infrastructure.Providers;

public class LogContextProvider : ILogContextProvider
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public LogContextProvider(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public string GetCorrelationId()
  {
    throw new NotImplementedException();
  }

  public string GetUserId()
  {
    return _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value
            ?? _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
  }
}