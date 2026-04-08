namespace ETicaretAPI.Common.SharedLibrary.Constants;

/// <summary>
/// Uygulama genelinde kullanılan sabit değerler.
/// </summary>
public static class AppConstants
{
  public static class Roles
  {
    public const string Admin = "Admin";
    public const string User = "User";
    public const string Seller = "Seller";
  }

  public static class CacheKeys
  {
    public const string AllProducts = "products:all";
    public const string AllCategories = "categories:all";
    public const string AllBrands = "brands:all";
    public const string ProductById = "products:id:{0}";
    public const string CategoryById = "categories:id:{0}";
    public const string BasketByUserId = "basket:user:{0}";
  }

  public static class EventNames
  {
    public const string OrderCreated = "OrderCreated";
    public const string PaymentCompleted = "PaymentCompleted";
    public const string PaymentFailed = "PaymentFailed";
    public const string StockUpdated = "StockUpdated";
    public const string BasketCheckedOut = "BasketCheckedOut";
  }
}
