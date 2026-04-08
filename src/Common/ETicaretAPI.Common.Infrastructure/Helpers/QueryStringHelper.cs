namespace ETicaretAPI.Common.Infrastructure.Helpers;

public static class QueryStringHelper
{
    public static string ToQueryString(object obj)
    {
        if (obj == null) return string.Empty;

        var properties = from p in obj.GetType().GetProperties()
                         where p.CanRead && p.GetIndexParameters().Length == 0 // Skip indexed properties
                         let value = p.GetValue(obj, null)
                         where value != null
                         select $"{Uri.EscapeDataString(p.Name)}={Uri.EscapeDataString(value.ToString() ?? "")}";

        return string.Join("&", properties);
    }
}
