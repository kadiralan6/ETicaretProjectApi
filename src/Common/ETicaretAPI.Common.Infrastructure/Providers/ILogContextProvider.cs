namespace ETicaretAPI.Common.Infrastructure.Providers;

public interface ILogContextProvider
{
    string GetUserId();
    string GetCorrelationId();
}