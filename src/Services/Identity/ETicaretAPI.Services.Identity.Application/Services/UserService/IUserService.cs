using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Services.Identity.Domain.DTOs;

namespace ETicaretAPI.Services.Identity.Application.Services.UserService;

public interface IUserService
{
    Task<ApiResponse<UserDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<List<UserDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<UserDto>> UpdateAsync(int userId, UpdateUserDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse> ChangePasswordAsync(int userId, ChangePasswordDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse> AssignRoleAsync(AssignRoleDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse> RemoveRoleAsync(int userId, string roleName, CancellationToken cancellationToken = default);
    Task<ApiResponse<List<string>>> GetRolesAsync(int userId, CancellationToken cancellationToken = default);
}
