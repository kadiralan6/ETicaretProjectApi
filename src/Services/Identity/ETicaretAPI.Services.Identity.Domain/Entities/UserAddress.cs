using ETicaretAPI.Common.Domain.Entities;

namespace ETicaretAPI.Services.Identity.Domain.Entities;

public class UserAddress : Entity<int>
{
    public int UserId { get; set; }
    public string? Title { get; set; }
    public string? City { get; set; }
    public string? FullAddress { get; set; }

    public AppUser? User { get; set; }
}
