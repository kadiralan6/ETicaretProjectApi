namespace ETicaretAPI.Common.Application.Exceptions;

/// <summary>
/// Kaynak bulunamadığında fırlatılır.
/// </summary>
public class NotFoundException : Exception
{
  public NotFoundException(string message) : base(message) { }
  public NotFoundException(string name, object key) : base($"{name} ({key}) bulunamadı.") { }
}

/// <summary>
/// Validasyon hatası olduğunda fırlatılır.
/// </summary>
public class ValidationException : Exception
{
  public List<string> Errors { get; }

  public ValidationException(List<string> errors) : base("Bir veya daha fazla validasyon hatası oluştu.")
  {
    Errors = errors;
  }
}

/// <summary>
/// İş kuralı ihlallerinde fırlatılır.
/// </summary>
public class BusinessRuleException : Exception
{
  public BusinessRuleException(string message) : base(message) { }
}

/// <summary>
/// Yetkilendirme hatalarında fırlatılır.
/// </summary>
public class UnauthorizedException : Exception
{
  public UnauthorizedException(string message = "Bu işlem için yetkiniz bulunmamaktadır.") : base(message) { }
}
