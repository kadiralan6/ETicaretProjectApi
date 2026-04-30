using ETicaretAPI.Common.Application.Exceptions;
using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Services.Identity.Domain.DTOs;
using ETicaretAPI.Services.Identity.Domain.Entities;
using ETicaretAPI.Services.Identity.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;

namespace ETicaretAPI.Services.Identity.Application.Services.UserService;

public class UserManager : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserManager(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<ApiResponse<UserDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetWithAsNoTrackingAsync(u => u.Id == id && !u.IsDeleted, cancellationToken: cancellationToken);
        if (user is null)
            throw new NotFoundException("Kullanıcı", id);

        return ApiResponse<UserDto>.Success(MapToDto(user));
    }

    public async Task<ApiResponse<List<UserDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllWithAsNoTrackingAsync(u => !u.IsDeleted, cancellationToken: cancellationToken);
        var dtos = users.Select(MapToDto).ToList();
        return ApiResponse<List<UserDto>>.Success(dtos);
    }

    public async Task<ApiResponse<UserDto>> UpdateAsync(int userId, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken: cancellationToken);
        if (user is null)
            throw new NotFoundException("Kullanıcı", userId);

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.PhoneNumber = dto.PhoneNumber;

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<UserDto>.Success(MapToDto(user));
    }

    public async Task<ApiResponse> ChangePasswordAsync(int userId, ChangePasswordDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken: cancellationToken);
        if (user is null)
            throw new NotFoundException("Kullanıcı", userId);

        var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, dto.CurrentPassword);
        if (verifyResult == PasswordVerificationResult.Failed)
            throw new ValidationException(["Mevcut şifre yanlış."]);

        if (dto.NewPassword != dto.ConfirmNewPassword)
            throw new ValidationException(["Yeni şifreler eşleşmiyor."]);

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success();
    }

    public async Task<ApiResponse> AssignRoleAsync(AssignRoleDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetAsync(u => u.Id == dto.UserId && !u.IsDeleted, cancellationToken: cancellationToken);
        if (user is null)
            throw new NotFoundException("Kullanıcı", dto.UserId);

        var roles = ParseRoles(user.Roles);
        if (!roles.Contains(dto.RoleName, StringComparer.OrdinalIgnoreCase))
        {
            roles.Add(dto.RoleName);
            user.Roles = string.Join(",", roles);
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return ApiResponse.Success();
    }

    public async Task<ApiResponse> RemoveRoleAsync(int userId, string roleName, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken: cancellationToken);
        if (user is null)
            throw new NotFoundException("Kullanıcı", userId);

        var roles = ParseRoles(user.Roles);
        if (roles.Remove(roleName))
        {
            user.Roles = string.Join(",", roles);
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return ApiResponse.Success();
    }

    public async Task<ApiResponse<List<string>>> GetRolesAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetWithAsNoTrackingAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken: cancellationToken);
        if (user is null)
            throw new NotFoundException("Kullanıcı", userId);

        return ApiResponse<List<string>>.Success(ParseRoles(user.Roles));
    }

    private static UserDto MapToDto(User user) => new()
    {
        Id = user.Id,
        Email = user.Email ?? string.Empty,
        UserName = user.UserName ?? string.Empty,
        FirstName = user.FirstName ?? string.Empty,
        LastName = user.LastName ?? string.Empty,
        IsActive = user.IsActive,
        Roles = ParseRoles(user.Roles)
    };

    private static List<string> ParseRoles(string roles) =>
        string.IsNullOrWhiteSpace(roles)
            ? ["User"]
            : [.. roles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)];
}
