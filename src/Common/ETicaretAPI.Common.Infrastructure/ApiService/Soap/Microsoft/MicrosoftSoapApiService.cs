using System.Reflection;

namespace ETicaretAPI.Common.Infrastructure.ApiService.Soap.Microsoft;

public class MicrosoftSoapApiService<TSoapClient> : ISoapApiService where TSoapClient : class
{
    private readonly TSoapClient _soapClient;

    public MicrosoftSoapApiService(TSoapClient soapClient)
    {
        _soapClient = soapClient ?? throw new ArgumentNullException(nameof(soapClient));
    }

    public Task<TResult> GetAsync<TResult>(string endpoint, object? queryParams = null)
    {
        throw new NotImplementedException("SOAP servislerde GET metodu desteklenmiyor.");
    }

    public Task<TResult> PostAsync<TResult>(string endpoint, object? body = null, string? methodName = null)
    {
        if (string.IsNullOrEmpty(methodName))
            throw new ArgumentNullException(nameof(methodName));

        var method = typeof(TSoapClient).GetMethod(methodName + "Async", BindingFlags.Public | BindingFlags.Instance);

        if (method == null)
            throw new NotImplementedException($"'{typeof(TSoapClient).Name}' içinde '{methodName}Async' metodu bulunamadı.");

        var parameters = method.GetParameters();

        object[] args;

        if (parameters.Length == 0)
        {
            // Parametresiz metod
            args = Array.Empty<object>();
        }
        else if (parameters.Length == 1)
        {
            // Tek parametre bekleniyor
            if (body == null)
                throw new ArgumentNullException(nameof(body), "Metod tek parametre bekliyor, body null olamaz.");

            var paramType = parameters[0].ParameterType;

            if (!paramType.IsInstanceOfType(body))
                throw new ArgumentException($"Parametre tipi '{paramType.Name}' bekleniyor, '{body.GetType().Name}' geldi.");

            args = new[] { body };
        }
        else
        {
            // Çoklu parametre bekleniyor
            if (body == null)
                throw new ArgumentNullException(nameof(body), "Metod birden fazla parametre bekliyor, body null olamaz.");

            if (body is not object[] arrBody)
                throw new ArgumentException("Body birden fazla parametre için object[] olmalıdır.");

            if (arrBody.Length != parameters.Length)
                throw new ArgumentException($"Metod {parameters.Length} parametre bekliyor, body'deki dizi uzunluğu {arrBody.Length}.");

            // Tip kontrolü yapabiliriz (isteğe bağlı)
            for (int i = 0; i < parameters.Length; i++)
            {
                if (arrBody[i] == null)
                {
                    if (parameters[i].ParameterType.IsValueType)
                        throw new ArgumentException($"Parametre {i} için null geçilemez.");
                    continue;
                }

                if (!parameters[i].ParameterType.IsInstanceOfType(arrBody[i]))
                    throw new ArgumentException($"Parametre {i} tipi '{parameters[i].ParameterType.Name}' bekleniyor, '{arrBody[i].GetType().Name}' geldi.");
            }

            args = arrBody;
        }

        dynamic task = method.Invoke(_soapClient, args);

        // Dönen task TResult tipinde Task olmalı, dönüşümü yap
        return (Task<TResult>)task;
    }
}