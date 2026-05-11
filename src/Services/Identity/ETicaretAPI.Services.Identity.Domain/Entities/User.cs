using ETicaretAPI.Common.Domain.Entities;

namespace ETicaretAPI.Services.Identity.Domain.Entities;

public class User : Entity<int>
{
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsActive { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public DateTime BirthDay { get; set; }
    public string? PhoneNumber { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    public string Roles { get; set; } = "User";

    // Navigation
    public ICollection<Address> Addresses { get; set; } = [];
}

