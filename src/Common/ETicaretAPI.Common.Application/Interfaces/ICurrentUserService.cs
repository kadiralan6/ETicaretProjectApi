namespace ETicaretAPI.Common.Application.Interfaces;

/// <summary>
/// Mevcut kullanıcı bilgilerini almak için.
/// </summary>
public interface ICurrentUserService
{
  string? UserId { get; }
  string? UserName { get; }
  string? Email { get; }
  IEnumerable<string> Roles { get; }
  bool IsAuthenticated { get; }
}
