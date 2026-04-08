using ETicaretAPI.Common.Application.Exceptions;
using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Services.Identity.Domain.DTOs;
using ETicaretAPI.Services.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Services.Identity.Application.Services.UserService;

public class UserManager : IUserService
{
    private readonly Microsoft.AspNetCore.Identity.UserManager<AppUser> _identityUserManager;

    public UserManager(Microsoft.AspNetCore.Identity.UserManager<AppUser> identityUserManager)
    {
        _identityUserManager = identityUserManager;
    }

    public async Task<ApiResponse<UserDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _identityUserManager.FindByIdAsync(id.ToString());
        if (user == null)
            throw new NotFoundException("Kullanici", id);

        var roles = await _identityUserManager.GetRolesAsync(user);
        return ApiResponse<UserDto>.Success(MapToDto(user, roles));
    }

    public async Task<ApiResponse<List<UserDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _identityUserManager.Users.ToListAsync(cancellationToken);
        var dtos = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _identityUserManager.GetRolesAsync(user);
            dtos.Add(MapToDto(user, roles));
        }

        return ApiResponse<List<UserDto>>.Success(dtos);
    }

    public async Task<ApiResponse<UserDto>> UpdateAsync(int userId, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _identityUserManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new NotFoundException("Kullanici", userId);

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;

        var result = await _identityUserManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new ValidationException(new List<string> { errors });
        }

        var roles = await _identityUserManager.GetRolesAsync(user);
        return ApiResponse<UserDto>.Success(MapToDto(user, roles));
    }

    public async Task<ApiResponse> ChangePasswordAsync(int userId, ChangePasswordDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _identityUserManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new NotFoundException("Kullanici", userId);

        var result = await _identityUserManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new ValidationException(new List<string> { errors });
        }

        return ApiResponse.Success();
    }

    public async Task<ApiResponse> AssignRoleAsync(AssignRoleDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _identityUserManager.FindByIdAsync(dto.UserId.ToString());
        if (user == null)
            throw new NotFoundException("Kullanici", dto.UserId);

        if (!await _identityUserManager.IsInRoleAsync(user, dto.RoleName))
        {
            var result = await _identityUserManager.AddToRoleAsync(user, dto.RoleName);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ValidationException(new List<string> { errors });
            }
        }

        return ApiResponse.Success();
    }

    public async Task<ApiResponse> RemoveRoleAsync(int userId, string roleName, CancellationToken cancellationToken = default)
    {
        var user = await _identityUserManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new NotFoundException("Kullanici", userId);

        var result = await _identityUserManager.RemoveFromRoleAsync(user, roleName);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new ValidationException(new List<string> { errors });
        }

        return ApiResponse.Success();
    }

    public async Task<ApiResponse<List<string>>> GetRolesAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _identityUserManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new NotFoundException("Kullanici", userId);

        var roles = await _identityUserManager.GetRolesAsync(user);
        return ApiResponse<List<string>>.Success(roles.ToList());
    }

    private static UserDto MapToDto(AppUser user, IList<string> roles) => new()
    {
        Id = user.Id,
        Email = user.Email ?? string.Empty,
        FirstName = user.FirstName ?? string.Empty,
        LastName = user.LastName ?? string.Empty,
        Roles = roles.ToList()
    };
}
