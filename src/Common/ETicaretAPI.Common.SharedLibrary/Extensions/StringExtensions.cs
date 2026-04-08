namespace ETicaretAPI.Common.SharedLibrary.Extensions;

/// <summary>
/// String extension metodları.
/// </summary>
public static class StringExtensions
{
  public static string ToSlug(this string text)
  {
    if (string.IsNullOrWhiteSpace(text))
      return string.Empty;

    text = text.ToLowerInvariant().Trim();
    text = text.Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u")
               .Replace("ş", "s").Replace("ö", "o").Replace("ç", "c")
               .Replace("İ", "i").Replace("Ğ", "g").Replace("Ü", "u")
               .Replace("Ş", "s").Replace("Ö", "o").Replace("Ç", "c");

    text = System.Text.RegularExpressions.Regex.Replace(text, @"[^a-z0-9\s-]", "");
    text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", "-");
    text = System.Text.RegularExpressions.Regex.Replace(text, @"-+", "-");
    text = text.Trim('-');

    return text;
  }

  public static bool IsValidEmail(this string email)
  {
    if (string.IsNullOrWhiteSpace(email))
      return false;

    try
    {
      var addr = new System.Net.Mail.MailAddress(email);
      return addr.Address == email;
    }
    catch
    {
      return false;
    }
  }
}
