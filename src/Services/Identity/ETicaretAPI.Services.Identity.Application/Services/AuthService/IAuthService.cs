using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Services.Identity.Domain.DTOs;

namespace ETicaretAPI.Services.Identity.Application.Services.AuthService;

public interface IAuthService
{
    Task<ApiResponse<TokenDto>> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<TokenDto>> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<TokenDto>> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse> LogoutAsync(int userId, CancellationToken cancellationToken = default);
}
