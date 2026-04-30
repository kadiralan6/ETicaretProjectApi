using ETicaretAPI.Services.Identity.Application.Services.AuthService;
using ETicaretAPI.Services.Identity.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Services.Identity.WebAPI.Controllers;

[ApiController]
[Route("api/identity/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _authService.LoginAsync(dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _authService.RegisterAsync(dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("refreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _authService.RefreshTokenAsync(dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("logout/{userId:int}")]
    public async Task<IActionResult> Logout(int userId, CancellationToken cancellationToken = default)
    {
        var result = await _authService.LogoutAsync(userId, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
