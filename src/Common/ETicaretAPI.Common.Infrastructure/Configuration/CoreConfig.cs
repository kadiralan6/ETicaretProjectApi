using Microsoft.Extensions.Configuration;

namespace ETicaretAPI.Common.Infrastructure.Configuration
{
    public class CoreConfig
    {
        public static IConfigurationBuilder? _configurationBuilder;
        public static IConfiguration? _configuration;

        public static IConfiguration GetConfiguration()
        {
            _configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                //.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json");
            _configuration = _configurationBuilder.Build();
            return _configuration;
        }

        public static T GetValue<T>(string key) => GetConfiguration().GetValue<T>(key);

        public static T GetSectionValue<T>(string key) => GetConfiguration().GetSection(key).Get<T>();

        public static string GetConnectionString(string connection) => GetConfiguration().GetConnectionString(connection);
    }
}
