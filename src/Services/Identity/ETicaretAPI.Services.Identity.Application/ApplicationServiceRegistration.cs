using ETicaretAPI.Services.Identity.Application.Services.AddressService;
using ETicaretAPI.Services.Identity.Application.Services.AuthService;
using ETicaretAPI.Services.Identity.Application.Services.TokenService;
using ETicaretAPI.Services.Identity.Application.Services.UserService;
using Microsoft.Extensions.DependencyInjection;

namespace ETicaretAPI.Services.Identity.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthManager>();
        services.AddScoped<IUserService, UserManager>();
        services.AddScoped<IAddressService, AddressManager>();
        services.AddScoped<ITokenService, TokenManager>();

        return services;
    }
}
