using ETicaretAPI.Common.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ETicaretAPI.Services.Identity.Domain.Entities;

public class User : Entity<int>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsActive { get; set; }
    public string? Email { get; set; }
    public DateTime BirthDay { get; set; }
    public string? PhoneNumber { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    public int? AddressId { get; set; }

}

