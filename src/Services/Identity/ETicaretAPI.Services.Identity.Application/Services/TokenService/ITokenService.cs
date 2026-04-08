using ETicaretAPI.Services.Identity.Domain.DTOs;

namespace ETicaretAPI.Services.Identity.Application.Services.TokenService;

public interface ITokenService
{
    Task<TokenDto> CreateTokenAsync(int userId, string email, string userName, IList<string> roles);
}
