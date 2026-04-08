using ETicaretAPI.Common.SharedLibrary.DTOs.Address;
using ETicaretAPI.Common.SharedLibrary.DTOs.Company;
using ETicaretAPI.Common.SharedLibrary.DTOs.Email;
using ETicaretAPI.Common.SharedLibrary.DTOs.User;
using ETicaretAPI.Common.SharedLibrary.Enums.Identity;

namespace ETicaretAPI.Common.Infrastructure.ApiService;

/// <summary>
/// Identity API calls with automatic caching support
/// </summary>
public interface IIdentityApiService
{
    // User Methods
    Task<List<GetUserCommonDto>> GetUsersByIdsAsync(List<Guid> userIds);
    Task<GetUserCommonDto> GetUserByIdAsync(Guid userId);
    Task<GetUserCommonDto> GetUserByUserCodeAsync(string userCode);
    Task<List<GetUserCommonDto>> GetUsersByCodesAsync(List<string> userCodes);
    Task<List<GetUserCommonDto>> GetUsersByCompanyIdsAsync(List<Guid> companyIds);
    Task<List<GetUserCommonDto>> GetUsersByCompanyIdAsync(Guid companyId);
    Task<string> GetUserCodeByIdAsync(Guid userId);
    Task<List<GetUserLookupDto>> GetUserLookupByIdsAsync(List<Guid> userIds);

    // Seller Methods
    Task<GetUserSellerInfoDto> GetUserSellerInfoAsync(Guid userId);
    Task<GetUserSellerInfoDto> GetSellerInfoByCodeAsync(string sellerCode);
    Task<GetUserSellerInfoDto> GetPlatformSellerInfoAsync();
    Task<GetUserCommonDto> GetSellerInformationForCartByCodeAsync(string sellerCode);

    // Company Methods
    Task<GetCompanyCommonDto> GetCompanyByCodeAsync(string companyCode);
    Task<GetCompanyCommonDto> GetCompanyByIdAsync(Guid companyId);
    Task<List<GetCompanyCommonDto>> GetCompaniesByCodesAsync(List<string> companyCodes);
    Task<List<GetCompanyCommonDto>> GetCompaniesByIdsAsync(List<Guid> companyIds);

    // Email Methods
    Task<EmailResponseDto> GetEmailByUserIdAsync(Guid userId);
    Task<List<EmailResponseDto>> GetEmailListByUserIdsAsync(List<Guid> userIds);

    // Address Methods
    Task<GetAddressDto> GetAddressByIdAsync(Guid addressId);
    Task<List<GetAddressDto>> GetAddressesByIdsAsync(List<Guid> addressIds);
    Task<List<GetAddressDto>> GetAddressByOwnerCodeAndTypeAsync(string ownerCode, UserTypeEnum userType);
    Task<List<GetAddressDto>> GetDefaultAddressesByOwnerIdAsync(Guid ownerId);

    // Platform Methods
    Task<List<string>> GetPlatformCompanyCodesAsync();
    Task<List<GetUserLookupDto>> GetIsPlatformUserByOwnerCodesAsync(List<string> ownerCodes);

    // Validation Methods
    Task<Guid> GetOwnerIdByUserCodeAsync(string userCode);
    Task<bool> IsUserIncludedInCompanyAsync(Guid userId, string companyCode);

    // Cache Invalidation
    Task InvalidateUserCacheAsync(Guid userId);
    Task InvalidateUserCacheByCodeAsync(string userCode);
    Task InvalidateCompanyCacheAsync(Guid companyId);
    Task InvalidateCompanyCacheByCodeAsync(string companyCode);
}
