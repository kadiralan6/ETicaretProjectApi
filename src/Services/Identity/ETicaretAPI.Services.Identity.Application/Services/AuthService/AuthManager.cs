using ETicaretAPI.Common.Application.Exceptions;
using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Services.Identity.Application.Services.TokenService;
using ETicaretAPI.Services.Identity.Domain.DTOs;
using ETicaretAPI.Services.Identity.Domain.Entities;
using ETicaretAPI.Services.Identity.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;

namespace ETicaretAPI.Services.Identity.Application.Services.AuthService;

public class AuthManager : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthManager(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<ApiResponse<TokenDto>> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetAsync(u => u.Email == dto.Email && !u.IsDeleted, cancellationToken: cancellationToken);
        if (user is null)
            throw new UnauthorizedException("Geçersiz email veya şifre.");

        if (!user.IsActive)
            throw new UnauthorizedException("Hesabınız aktif değil.");

        var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, dto.Password);
        if (verifyResult == PasswordVerificationResult.Failed)
            throw new UnauthorizedException("Geçersiz email veya şifre.");

        var roles = ParseRoles(user.Roles);
        var token = await _tokenService.CreateTokenAsync(user.Id, user.Email!, user.UserName ?? user.Email!, roles);

        user.RefreshToken = token.RefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        FillUserInfo(token, user, roles);
        return ApiResponse<TokenDto>.Success(token);
    }

    public async Task<ApiResponse<TokenDto>> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository.GetWithAsNoTrackingAsync(u => u.Email == dto.Email && !u.IsDeleted, cancellationToken: cancellationToken);
        if (existingUser is not null)
            throw new BusinessRuleException("Bu email adresi zaten kullanılıyor.");

        var user = new User
        {
            UserName = string.IsNullOrWhiteSpace(dto.UserName) ? dto.Email : dto.UserName,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            IsActive = true,
            Roles = "User"
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var roles = ParseRoles(user.Roles);
        var token = await _tokenService.CreateTokenAsync(user.Id, user.Email!, user.UserName!, roles);

        user.RefreshToken = token.RefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        FillUserInfo(token, user, roles);
        return ApiResponse<TokenDto>.Success(token, statusCode: 201);
    }

    public async Task<ApiResponse<TokenDto>> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetAsync(u => u.RefreshToken == dto.RefreshToken && !u.IsDeleted, cancellationToken: cancellationToken);
        if (user is null || user.RefreshTokenExpiry <= DateTime.UtcNow)
            throw new UnauthorizedException("Geçersiz veya süresi dolmuş refresh token.");

        var roles = ParseRoles(user.Roles);
        var token = await _tokenService.CreateTokenAsync(user.Id, user.Email!, user.UserName ?? user.Email!, roles);

        user.RefreshToken = token.RefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        FillUserInfo(token, user, roles);
        return ApiResponse<TokenDto>.Success(token);
    }

    public async Task<ApiResponse> LogoutAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken: cancellationToken);
        if (user is null)
            throw new NotFoundException("Kullanıcı", userId);

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success();
    }

    private static void FillUserInfo(TokenDto token, User user, List<string> roles)
    {
        token.UserId = user.Id;
        token.Email = user.Email ?? string.Empty;
        token.UserName = user.UserName ?? string.Empty;
        token.FirstName = user.FirstName ?? string.Empty;
        token.LastName = user.LastName ?? string.Empty;
        token.Roles = roles;
    }

    private static List<string> ParseRoles(string roles) =>
        string.IsNullOrWhiteSpace(roles)
            ? new List<string> { "User" }
            : roles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
}
