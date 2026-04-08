namespace ETicaretAPI.Services.Identity.Domain.DTOs;

public class AssignRoleDto
{
    public int UserId { get; set; }
    public string RoleName { get; set; } = string.Empty;
}
