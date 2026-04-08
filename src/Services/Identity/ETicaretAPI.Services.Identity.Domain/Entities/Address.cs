using ETicaretAPI.Common.Domain.Entities;

namespace ETicaretAPI.Services.Identity.Domain.Entities;

public class Address : Entity<int>
{
    public string? Title { get; set; }
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? FullAddress { get; set; }
    public string? PostalCode { get; set; }
    public bool IsDefault { get; set; }

}
