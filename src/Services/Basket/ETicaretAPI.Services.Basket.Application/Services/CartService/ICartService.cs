using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Services.Basket.Domain.DTOs.CartDtos;

namespace ETicaretAPI.Services.Basket.Application.Services.CartService;

public interface ICartService
{
    /// <summary>
    /// Oturumdaki kullanıcının sepetini getirir. Sepet yoksa yeni oluşturur.
    /// </summary>
    Task<ApiResponse<GetCartDto>> GetOrCreateCartAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// ID ile sepeti getirir.
    /// </summary>
    Task<ApiResponse<GetCartDto>> GetByIdAsync(int cartId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Admin için sepetleri filtreli ve sayfalı getirir.
    /// </summary>
    Task<ApiResponse<PagedResult<GetCartDto>>> GetCartsFilterAsync(GetCartForAdminFilterDto filterDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sepete ürün ekler. Aynı ürün varsa miktarını artırır.
    /// </summary>
    Task<ApiResponse<GetCartDto>> AddItemAsync(int userId, AddCartItemDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sepetteki ürünün miktarını günceller. Miktar 0 ise ürünü sepetten kaldırır.
    /// </summary>
    Task<ApiResponse<GetCartDto>> UpdateItemAsync(int userId, UpdateCartItemDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sepetten belirli bir ürünü kaldırır.
    /// </summary>
    Task<ApiResponse<bool>> RemoveItemAsync(int userId, int cartItemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sepete kupon kodu uygular ve indirimi hesaplar.
    /// </summary>
    Task<ApiResponse<GetCartDto>> ApplyCouponAsync(int userId, ApplyCouponDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sepetten kuponu kaldırır.
    /// </summary>
    Task<ApiResponse<GetCartDto>> RemoveCouponAsync(int userId, int cartId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sepeti tamamen temizler (tüm ürünleri kaldırır).
    /// </summary>
    Task<ApiResponse<bool>> ClearCartAsync(int userId, CancellationToken cancellationToken = default);
}
