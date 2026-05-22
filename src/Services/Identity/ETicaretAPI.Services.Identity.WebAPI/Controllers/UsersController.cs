using ETicaretAPI.Services.Identity.Application.Services.UserService;
using ETicaretAPI.Services.Identity.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Services.Identity.WebAPI.Controllers;

[ApiController]
[Route("api/identity/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("getAll")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetAllAsync(cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("getById/{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetByIdAsync(id, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("update/{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _userService.UpdateAsync(id, dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("changePassword/{id:int}")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _userService.ChangePasswordAsync(id, dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("assignRole")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _userService.AssignRoleAsync(dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("removeRole/{userId:int}/{roleName}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveRole(int userId, string roleName, CancellationToken cancellationToken = default)
    {
        var result = await _userService.RemoveRoleAsync(userId, roleName, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("getRoles/{id:int}")]
    public async Task<IActionResult> GetRoles(int id, CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetRolesAsync(id, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
