using Microsoft.AspNetCore.Identity;

namespace ETicaretAPI.Services.Identity.Domain.Entities;

public class AppUser : IdentityUser<int>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }

    public ICollection<UserAddress> Addresses { get; set; } = new List<UserAddress>();
}
