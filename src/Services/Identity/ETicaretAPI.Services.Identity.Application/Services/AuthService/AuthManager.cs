using ETicaretAPI.Common.Application.Exceptions;
using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Services.Identity.Application.Services.TokenService;
using ETicaretAPI.Services.Identity.Domain.DTOs;
using ETicaretAPI.Services.Identity.Domain.Entities;

namespace ETicaretAPI.Services.Identity.Application.Services.AuthService;

public class AuthManager : IAuthService
{
    private readonly Microsoft.AspNetCore.Identity.UserManager<AppUser> _userManager;
    private readonly Microsoft.AspNetCore.Identity.SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;

    public AuthManager(
        Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager,
        Microsoft.AspNetCore.Identity.SignInManager<AppUser> signInManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public async Task<ApiResponse<TokenDto>> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            throw new UnauthorizedException("Gecersiz email veya sifre.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, true);
        if (result.IsLockedOut)
            throw new BusinessRuleException("Hesabiniz kilitlendi. Lutfen daha sonra tekrar deneyin.");
        if (!result.Succeeded)
            throw new UnauthorizedException("Gecersiz email veya sifre.");

        var roles = await _userManager.GetRolesAsync(user);
        var token = await _tokenService.CreateTokenAsync(user.Id, user.Email!, user.UserName!, roles);

        user.RefreshToken = token.RefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return ApiResponse<TokenDto>.Success(token);
    }

    public async Task<ApiResponse<TokenDto>> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            throw new BusinessRuleException("Bu email adresi zaten kullaniliyor.");

        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new ValidationException(new List<string> { errors });
        }

        await _userManager.AddToRoleAsync(user, "User");

        var roles = await _userManager.GetRolesAsync(user);
        var token = await _tokenService.CreateTokenAsync(user.Id, user.Email!, user.UserName!, roles);

        user.RefreshToken = token.RefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return ApiResponse<TokenDto>.Success(token, statusCode: 201);
    }

    public async Task<ApiResponse<TokenDto>> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.RefreshToken == dto.RefreshToken);
        if (user == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
            throw new UnauthorizedException("Gecersiz veya suresi dolmus refresh token.");

        var roles = await _userManager.GetRolesAsync(user);
        var token = await _tokenService.CreateTokenAsync(user.Id, user.Email!, user.UserName!, roles);

        user.RefreshToken = token.RefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return ApiResponse<TokenDto>.Success(token);
    }

    public async Task<ApiResponse> LogoutAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new NotFoundException("Kullanici", userId);

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        await _userManager.UpdateAsync(user);

        return ApiResponse.Success();
    }
}
