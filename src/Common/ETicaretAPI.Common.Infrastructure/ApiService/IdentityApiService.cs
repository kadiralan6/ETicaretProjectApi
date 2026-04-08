using Microsoft.Extensions.Configuration;
using ETicaretAPI.Common.Application.Constants;
using ETicaretAPI.Common.Infrastructure.Cache;
using ETicaretAPI.Common.Infrastructure.Configuration;
using ETicaretAPI.Common.SharedLibrary.DTOs.Address;
using ETicaretAPI.Common.SharedLibrary.DTOs.Company;
using ETicaretAPI.Common.SharedLibrary.DTOs.Email;
using ETicaretAPI.Common.SharedLibrary.DTOs.User;
using ETicaretAPI.Common.SharedLibrary.Enums.Identity;

namespace ETicaretAPI.Common.Infrastructure.ApiService;

public class IdentityApiService : IIdentityApiService
{
    private readonly ICacheService _cache;
    private readonly IRestApiService _apiService;
    private readonly string _identityApiUrl;

    // Cache TTL configurations - Optimized for rarely changing identity data
    private static readonly TimeSpan UserCacheTTL = TimeSpan.FromHours(8);
    private static readonly TimeSpan CompanyCacheTTL = TimeSpan.FromHours(24);
    private static readonly TimeSpan EmailCacheTTL = TimeSpan.FromHours(24);
    private static readonly TimeSpan SellerInfoCacheTTL = TimeSpan.FromHours(12);
    private static readonly TimeSpan LookupCacheTTL = TimeSpan.FromHours(12);
    private static readonly TimeSpan AddressCacheTTL = TimeSpan.FromHours(6);
    private static readonly TimeSpan PlatformCacheTTL = TimeSpan.FromHours(24);

    public IdentityApiService(
        ICacheService cache,
        IRestApiService apiService)
    {
        _cache = cache;
        _apiService = apiService;
        _identityApiUrl = CoreConfig.GetValue<string>("ExternalApi:IdentityApi:Url");
    }

    #region User Methods

    public async Task<GetUserCommonDto> GetUserByIdAsync(Guid userId)
    {
        var cacheKey = GetUserCacheKey(userId);
        Console.WriteLine($"[CACHE CHECK] Key: {cacheKey}");

        return await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            Console.WriteLine($"[CACHE MISS] Calling API for key: {cacheKey}");
            var url = $"{_identityApiUrl}{string.Format(IdentityApiEndpoints.GetByIdForCommonUserDto, userId)}";
            var result = await _apiService.GetDataResultAsync<List<GetUserCommonDto>>(url);
            return result?.FirstOrDefault()!;
        }, UserCacheTTL);
    }

    public async Task<List<GetUserCommonDto>> GetUsersByIdsAsync(List<Guid> userIds)
    {
        if (userIds == null || !userIds.Any())
            return new List<GetUserCommonDto>();

        var results = new List<GetUserCommonDto>();
        var missingIds = new List<Guid>();

        // Check cache for each user
        foreach (var userId in userIds.Distinct())
        {
            var cacheKey = GetUserCacheKey(userId);
            Console.WriteLine($"[CACHE CHECK] Key: {cacheKey}");
            var cached = await _cache.GetAsync<GetUserCommonDto>(cacheKey);

            if (cached != null)
            {
                Console.WriteLine($"[CACHE HIT] Key: {cacheKey}");
                results.Add(cached);
            }
            else
            {
                Console.WriteLine($"[CACHE MISS] Key: {cacheKey}");
                missingIds.Add(userId);
            }
        }

        // Fetch missing users from API
        if (missingIds.Any())
        {
            var url = $"{_identityApiUrl}{IdentityApiEndpoints.GetUsersByIds}?id={string.Join("&id=", missingIds)}";
            var freshUsers = await _apiService.GetDataResultAsync<List<GetUserCommonDto>>(url);

            if (freshUsers != null)
            {
                foreach (var user in freshUsers)
                {
                    var cacheKey = GetUserCacheKey(user.Id);
                    await _cache.SetAsync(cacheKey, user, UserCacheTTL);
                    results.Add(user);
                }
            }
        }

        return results;
    }

    public async Task<GetUserCommonDto> GetUserByUserCodeAsync(string userCode)
    {
        var cacheKey = GetUserCodeCacheKey(userCode);
        Console.WriteLine($"[CACHE CHECK] Key: {cacheKey}");

        return await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            Console.WriteLine($"[CACHE MISS] Calling API for key: {cacheKey}");
            var url = $"{_identityApiUrl}{string.Format(IdentityApiEndpoints.GetUserByUserCode, userCode)}";
            return await _apiService.GetDataResultAsync<GetUserCommonDto>(url);
        }, UserCacheTTL);
    }

    public async Task<List<GetUserLookupDto>> GetUserLookupByIdsAsync(List<Guid> userIds)
    {
        if (userIds == null || !userIds.Any())
            return new List<GetUserLookupDto>();

        var cacheKey = GetUserLookupCacheKey(userIds);

        Console.WriteLine($"[CACHE CHECK] Key: {cacheKey}");

        return await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            Console.WriteLine($"[CACHE MISS] Calling API for key: {cacheKey}");
            var url = $"{_identityApiUrl}{IdentityApiEndpoints.GetUserLookupByIds}";
            return await _apiService.PostDataResultAsync<List<GetUserLookupDto>>(url, userIds);
        }, LookupCacheTTL);
    }

    public async Task<List<GetUserCommonDto>> GetUsersByCodesAsync(List<string> userCodes)
    {
        if (userCodes == null || !userCodes.Any())
            return new List<GetUserCommonDto>();

        var results = new List<GetUserCommonDto>();
        var missingCodes = new List<string>();

        // Check cache for each user code
        foreach (var userCode in userCodes.Distinct())
        {
            var cacheKey = GetUserCodeCacheKey(userCode);
            Console.WriteLine($"[CACHE CHECK] Key: {cacheKey}");
            var cached = await _cache.GetAsync<GetUserCommonDto>(cacheKey);
            if (cached != null)
            {
                Console.WriteLine($"[CACHE HIT] Key: {cacheKey}");
                results.Add(cached);
            }
            else
            {
                Console.WriteLine($"[CACHE MISS] Key: {cacheKey}");
                missingCodes.Add(userCode);
            }
        }

        // Fetch missing users from API
        if (missingCodes.Any())
        {
            var url = $"{_identityApiUrl}{IdentityApiEndpoints.GetUsersByCodes}";
            var freshUsers = await _apiService.PostDataResultAsync<List<GetUserCommonDto>>(url, missingCodes);

            if (freshUsers != null && freshUsers.Any())
            {
                foreach (var user in freshUsers)
                {
                    if (!string.IsNullOrEmpty(user.UserCode))
                    {
                        var cacheKey = GetUserCodeCacheKey(user.UserCode);
                        await _cache.SetAsync(cacheKey, user, UserCacheTTL);
                    }
                    results.Add(user);
                }
            }
        }

        return results;
    }

    public async Task<string> GetUserCodeByIdAsync(Guid userId)
    {
        var cacheKey = $"identity:usercode:{userId}";

        Console.WriteLine($"[CACHE CHECK] Key: {cacheKey}");

        return await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            Console.WriteLine($"[CACHE MISS] Calling API for key: {cacheKey}");
            var url = $"{_identityApiUrl}{string.Format(IdentityApiEndpoints.GetUserCodeById, userId)}";
            return await _apiService.GetDataResultAsync<string>(url);
        }, UserCacheTTL);
    }

    public async Task<List<GetUserCommonDto>> GetUsersByCompanyIdsAsync(List<Guid> companyIds)
    {
        if (companyIds == null || !companyIds.Any())
            return new List<GetUserCommonDto>();

        var cacheKey = $"identity:users:company:{string.Join("-", companyIds.OrderBy(x => x))}";

        Console.WriteLine($"[CACHE CHECK] Key: {cacheKey}");

        return await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            Console.WriteLine($"[CACHE MISS] Calling API for key: {cacheKey}");
            var url = $"{_identityApiUrl}{IdentityApiEndpoints.GetUsersByCompanyIds}";
            return await _apiService.PostDataResultAsync<List<GetUserCommonDto>>(url, companyIds);
        }, UserCacheTTL);
    }

    public async Task<List<GetUserCommonDto>> GetUsersByCompanyIdAsync(Guid companyId)
    {
        var cacheKey = $"identity:users:company:{companyId}";

        Console.WriteLine($"[CACHE CHECK] Key: {cacheKey}");

        return await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            Console.WriteLine($"[CACHE MISS] Calling API for key: {cacheKey}");
            var url = $"{_identityApiUrl}{string.Format(IdentityApiEndpoints.GetUsersByCompanyId, companyId)}";
            return await _apiService.GetDataResultAsync<List<GetUserCommonDto>>(url);
        }, UserCacheTTL);
    }

    #endregion

    #region Seller Methods

    public async Task<GetUserSellerInfoDto> GetUserSellerInfoAsync(Guid userId)
    {
        var cacheKey = GetSellerInfoCacheKey(userId);
        Console.WriteLine($"[CACHE CHECK] Key: {cacheKey}");

        return await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            Console.WriteLine($"[CACHE MISS] Calling API for key: {cacheKey}");
            var url = $"{_identityApiUrl}{string.Format(IdentityApiEndpoints.GetUserSellerInfo, userId)}";
            return await _apiService.GetDataResultAsync<GetUserSellerInfoDto>(url);
        }, SellerInfoCacheTTL);
    }

    public async Task<GetUserSellerInfoDto> GetSellerInfoByCodeAsync(string sellerCode)
    {
        var cacheKey = GetSellerCodeCacheKey(sellerCode);
        Console.WriteLine($"[CACHE CHECK] Key: {cacheKey}");

        return await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            Console.WriteLine($"[CACHE MISS] Calling API for key: {cacheKey}");
            var url = $"{_identityApiUrl}{string.Format(IdentityApiEndpoints.GetSellerInfoByCode, sellerCode)}";
            return await _apiService.GetDataResultAsync<GetUserSellerInfoDto>(url);
        }, SellerInfoCacheTTL);
    }

    public async Task<GetUserSellerInfoDto> GetPlatformSellerInfoAsync()
    {
        const string cacheKey = "identity:platform:sellerinfo";
        Console.WriteLine($"[CACHE CHECK] Key: {cacheKey}");

        return await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            Console.WriteLine($"[CACHE MISS] Calling API for key: {cacheKey}");
            var url = $"{_identityApiUrl}{IdentityApiEndpoints.GetPlatformSellerInfo}";
            return await _apiService.GetDataResultAsync<GetUserSellerInfoDto>(url);
        }, PlatformCacheTTL);
    }

    public async Task<GetUserCommonDto> GetSellerInformationForCartByCodeAsync(string sellerCode)
    {
        var cacheKey = GetSellerCodeCacheKey($"cart:{sellerCode}");
        Console.WriteLine($"[CACHE CHECK] Key: {cacheKey}");

        return await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            Console.WriteLine($"[CACHE MISS] Calling API for key: {cacheKey}");
            var url = $"{_identityApiUrl}{string.Format(IdentityApiEndpoints.GetSellerInformationForCartByCode, sellerCode)}";
            return await _apiService.GetDataResultAsync<GetUserCommonDto>(url);
        }, SellerInfoCacheTTL);
    }

    #endregion

    #region Company Methods

    public async Task<GetCompanyCommonDto> GetCompanyByCodeAsync(string companyCode)
    {
        var cacheKey = GetCompanyCodeCacheKey(companyCode);
        Console.WriteLine($"[CACHE CHECK] Key: {cacheKey}");

        return await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            Console.WriteLine($"[CACHE MISS] Calling API for key: {cacheKey}");
            var url = $"{_identityApiUrl}{string.Format(IdentityApiEndpoints.GetCompanyByCode, companyCode)}";
            return await _apiService.GetDataResultAsync<GetCompanyCommonDto>(url);
        }, CompanyCacheTTL);
    }

    public async Task<GetCompanyCommonDto> GetCompanyByIdAsync(Guid companyId)
    {
        var cacheKey = GetCompanyCacheKey(companyId);
        Console.WriteLine($"[CACHE CHECK] Key: {cacheKey}");

        return await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            Console.WriteLine($"[CACHE MISS] Calling API for key: {cacheKey}");
            var url = $"{_identityApiUrl}{string.Format(IdentityApiEndpoints.GetCompanyById, companyId)}";
            return await _apiService.GetDataResultAsync<GetCompanyCommonDto>(url);
        }, CompanyCacheTTL);
    }

    public async Task<List<GetCompanyCommonDto>> GetCompaniesByCodesAsync(List<string> companyCodes)
    {
        if (companyCodes == null || !companyCodes.Any())
            return new List<GetCompanyCommonDto>();

        var results = new List<GetCompanyCommonDto>();
        var missingCodes = new List<string>();

        // Check cache for each company
        foreach (var code in companyCodes.Distinct())
        {
            var cacheKey = GetCompanyCodeCacheKey(code);
            Console.WriteLine($"[CACHE CHECK] Key: {cacheKey}");
            var cached = await _cache.GetAsync<GetCompanyCommonDto>(cacheKey);

            if (cached != null)
            {
                Console.WriteLine($"[CACHE HIT] Key: {cacheKey}");
                results.Add(cached);
            }
            else
            {
                Console.WriteLine($"[CACHE MISS] Key: {cacheKey}");
                missingCodes.Add(code);
            }
        }

        // Fetch missing companies from API
        if (missingCodes.Any())
        {
            var url = $"{_identityApiUrl}{IdentityApiEndpoints.GetCompaniesByCodes}";
            var freshCompanies = await _apiService.PostDataResultAsync<List<GetCompanyCommonDto>>(url, missingCodes);

            if (freshCompanies != null)
            {
                foreach (var company in freshCompanies)
                {
                    var cacheKey = GetCompanyCodeCacheKey(company.CompanyCode);
                    await _cache.SetAsync(cacheKey, company, CompanyCacheTTL);
                    results.Add(company);
                }
            }
        }

        return results;
    }

    public async Task<List<GetCompanyCommonDto>> GetCompaniesByIdsAsync(List<Guid> companyIds)
    {
        if (companyIds == null || !companyIds.Any())
            return new List<GetCompanyCommonDto>();

        var results = new List<GetCompanyCommonDto>();
        var missingIds = new List<Guid>();

        // Check cache for each company
        foreach (var id in companyIds.Distinct())
        {
            var cacheKey = GetCompanyCacheKey(id);
            Console.WriteLine($"[CACHE CHECK] Key: {cacheKey}");
            var cached = await _cache.GetAsync<GetCompanyCommonDto>(cacheKey);

            if (cached != null)
            {
                Console.WriteLine($"[CACHE HIT] Key: {cacheKey}");
                results.Add(cached);
            }
            else
            {
                Console.WriteLine($"[CACHE MISS] Key: {cacheKey}");
                missingIds.Add(id);
            }
        }

        // Fetch missing companies from API
        if (missingIds.Any())
        {
            var url = $"{_identityApiUrl}{IdentityApiEndpoints.GetCompaniesByIds}";
            var freshCompanies = await _apiService.PostDataResultAsync<List<GetCompanyCommonDto>>(url, missingIds);

            if (freshCompanies != null)
            {
                foreach (var company in freshCompanies)
                {
                    var cacheKey = GetCompanyCacheKey(company.Id);
                    Console.WriteLine($"[CACHE MISS] Key: {cacheKey}");
                    await _cache.SetAsync(cacheKey, company, CompanyCacheTTL);
                    results.Add(company);
                }
            }
        }

        return results;
    }

    #endregion

    #region Email Methods

    public async Task<EmailResponseDto> GetEmailByUserIdAsync(Guid userId)
    {
        var cacheKey = GetEmailCacheKey(userId);

        Console.WriteLine($"[CACHE CHECK] Key: {cacheKey}");

        return await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            Console.WriteLine($"[CACHE MISS] Calling API for userId: {userId}");
            var url = $"{_identityApiUrl}{string.Format(IdentityApiEndpoints.GetEmailByUserId, userId)}";
            return await _apiService.GetDataResultAsync<EmailResponseDto>(url);
        }, EmailCacheTTL);
    }

    public async Task<List<EmailResponseDto>> GetEmailListByUserIdsAsync(List<Guid> userIds)
    {
        if (userIds == null || !userIds.Any())
            return new List<EmailResponseDto>();

        var results = new List<EmailResponseDto>();
        var missingIds = new List<Guid>();

        // Check cache for each email
        foreach (var userId in userIds.Distinct())
        {
            var cacheKey = GetEmailCacheKey(userId);
            Console.WriteLine($"[CACHE CHECK] Key: {cacheKey}");
            var cached = await _cache.GetAsync<EmailResponseDto>(cacheKey);

            if (cached != null)
            {
                Console.WriteLine($"[CACHE HIT] Key: {cacheKey}");
                results.Add(cached);
            }
            else
                missingIds.Add(userId);
        }

        // Fetch missing emails from API
        if (missingIds.Any())
        {
            var url = $"{_identityApiUrl}{IdentityApiEndpoints.GetEmailListByUserIds}";
            var freshEmails = await _apiService.PostDataResultAsync<List<EmailResponseDto>>(url, missingIds);

            if (freshEmails != null)
            {
                foreach (var email in freshEmails)
                {
                    if (email.UserId.HasValue)
                    {
                        var cacheKey = GetEmailCacheKey(email.UserId.Value);
                        Console.WriteLine($"[CACHE MISS] Key: {cacheKey}");
                        await _cache.SetAsync(cacheKey, email, EmailCacheTTL);
                    }
                    results.Add(email);
                }
            }
        }

        return results;
    }

    #endregion

    #region Address Methods

    public async Task<GetAddressDto> GetAddressByIdAsync(Guid addressId)
    {
        var cacheKey = GetAddressCacheKey(addressId);
        Console.WriteLine($"[CACHE CHECK] Key: {cacheKey}");

        return await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            Console.WriteLine($"[CACHE MISS] Calling API for key: {cacheKey}");
            var url = $"{_identityApiUrl}{string.Format(IdentityApiEndpoints.GetAddressById, addressId)}";
            return await _apiService.GetDataResultAsync<GetAddressDto>(url);
        }, AddressCacheTTL);
    }

    public async Task<List<GetAddressDto>> GetAddressesByIdsAsync(List<Guid> addressIds)
    {
        if (addressIds == null || !addressIds.Any())
            return new List<GetAddressDto>();

        var results = new List<GetAddressDto>();
        var missingIds = new List<Guid>();

        // Check cache for each address
        foreach (var addressId in addressIds.Distinct())
        {
            var cached = await _cache.GetAsync<GetAddressDto>(GetAddressCacheKey(addressId));
            var cacheKey = GetAddressCacheKey(addressId);
            Console.WriteLine($"[CACHE CHECK] Key: {cacheKey}");
            if (cached != null)
            {
                Console.WriteLine($"[CACHE HIT] Key: {cacheKey}");
                results.Add(cached);
            }
            else
            {
                Console.WriteLine($"[CACHE MISS] Key: {cacheKey}");
                missingIds.Add(addressId);
            }
        }

        // Fetch missing addresses from API
        if (missingIds.Any())
        {
            var url = $"{_identityApiUrl}{IdentityApiEndpoints.GetAddressesByIds}";
            var freshAddresses = await _apiService.PostDataResultAsync<List<GetAddressDto>>(url, missingIds);

            if (freshAddresses != null && freshAddresses.Any())
            {
                foreach (var address in freshAddresses)
                {
                    var cacheKey = GetAddressCacheKey(address.Id);
                    await _cache.SetAsync(cacheKey, address, AddressCacheTTL);
                    results.Add(address);
                }
            }
        }

        return results;
    }

    public async Task<List<GetAddressDto>> GetAddressByOwnerCodeAndTypeAsync(string ownerCode, UserTypeEnum userType)
    {
        var cacheKey = GetAddressOwnerCacheKey(ownerCode, userType);

        Console.WriteLine($"[CACHE CHECK] Key: {cacheKey}");

        return await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            Console.WriteLine($"[CACHE MISS] Calling API for key: {cacheKey}");
            var url = $"{_identityApiUrl}{string.Format(IdentityApiEndpoints.GetAddressByOwnerCodeAndType, ownerCode, userType)}";
            return await _apiService.GetDataResultAsync<List<GetAddressDto>>(url);
        }, AddressCacheTTL);
    }

    public async Task<List<GetAddressDto>> GetDefaultAddressesByOwnerIdAsync(Guid ownerId)
    {
        var cacheKey = $"identity:address:default:{ownerId}";
        Console.WriteLine($"[CACHE CHECK] Key: {cacheKey}");

        return await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            Console.WriteLine($"[CACHE MISS] Calling API for key: {cacheKey}");
            var url = $"{_identityApiUrl}{string.Format(IdentityApiEndpoints.GetDefaultAddressesByOwnerId, ownerId)}";
            return await _apiService.GetDataResultAsync<List<GetAddressDto>>(url);
        }, AddressCacheTTL);
    }

    #endregion

    #region Platform Methods

    public async Task<List<string>> GetPlatformCompanyCodesAsync()
    {
        const string cacheKey = "identity:platform:companycodes";
        Console.WriteLine($"[CACHE CHECK] Key: {cacheKey}");

        return await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            Console.WriteLine($"[CACHE MISS] Calling API for key: {cacheKey}");
            var url = $"{_identityApiUrl}{IdentityApiEndpoints.GetPlatformCompanyCodes}";
            return await _apiService.GetDataResultAsync<List<string>>(url);
        }, PlatformCacheTTL);
    }

    #endregion

    #region Cache Invalidation

    public async Task InvalidateUserCacheAsync(Guid userId)
    {
        await _cache.RemoveAsync(GetUserCacheKey(userId));
        await _cache.RemoveAsync(GetSellerInfoCacheKey(userId));
        await _cache.RemoveAsync(GetEmailCacheKey(userId));
    }

    public async Task InvalidateUserCacheByCodeAsync(string userCode)
    {
        await _cache.RemoveAsync(GetUserCodeCacheKey(userCode));
        await _cache.RemoveAsync(GetSellerCodeCacheKey(userCode));
    }

    public async Task InvalidateCompanyCacheAsync(Guid companyId)
    {
        await _cache.RemoveAsync(GetCompanyCacheKey(companyId));
    }

    public async Task InvalidateCompanyCacheByCodeAsync(string companyCode)
    {
        await _cache.RemoveAsync(GetCompanyCodeCacheKey(companyCode));
    }

    public async Task<Guid> GetOwnerIdByUserCodeAsync(string userCode)
    {
        var cacheKey = $"identity:owner:usercode:{userCode}";

        Console.WriteLine($"[CACHE CHECK] Key: {cacheKey}");

        return await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            Console.WriteLine($"[CACHE MISS] Calling API for key: {cacheKey}");
            var url = $"{_identityApiUrl}{string.Format(IdentityApiEndpoints.GetOwnerIdByUserCode, userCode)}";
            return await _apiService.GetDataResultAsync<Guid>(url);
        }, UserCacheTTL);
    }

    public async Task<bool> IsUserIncludedInCompanyAsync(Guid userId, string companyCode)
    {
        var url = $"{_identityApiUrl}{IdentityApiEndpoints.IsUserIncludedInCompany}";
        var requestBody = new { UserId = userId, CompanyCode = companyCode };
        var result = await _apiService.PostDataResultAsync<bool>(url, requestBody);
        return result;
    }

    public async Task<List<GetUserLookupDto>> GetIsPlatformUserByOwnerCodesAsync(List<string> ownerCodes)
    {
        if (ownerCodes == null || !ownerCodes.Any())
            return new List<GetUserLookupDto>();

        var results = new List<GetUserLookupDto>();
        var missingCodes = new List<string>();

        // Check cache for each owner code
        foreach (var ownerCode in ownerCodes.Distinct())
        {
            var cacheKey = $"identity:platform:owner:{ownerCode}";
            Console.WriteLine($"[CACHE CHECK] Key: {cacheKey}");
            var cached = await _cache.GetAsync<GetUserLookupDto>(cacheKey);

            if (cached != null)
            {
                Console.WriteLine($"[CACHE HIT] Key: {cacheKey}");
                results.Add(cached);
            }
            else
            {
                Console.WriteLine($"[CACHE MISS] Key: {cacheKey}");
                missingCodes.Add(ownerCode);
            }
        }

        // Fetch missing codes from API
        if (missingCodes.Any())
        {
            var url = $"{_identityApiUrl}{IdentityApiEndpoints.GetIsPlatformUserByOwnerCodes}";
            var freshLookups = await _apiService.PostDataResultAsync<List<GetUserLookupDto>>(url, missingCodes);

            if (freshLookups != null)
            {
                foreach (var lookup in freshLookups)
                {
                    var cacheKey = $"identity:platform:owner:{lookup.UserCode}";
                    Console.WriteLine($"[CACHE MISS] Key: {cacheKey}");
                    await _cache.SetAsync(cacheKey, lookup, PlatformCacheTTL);
                    results.Add(lookup);
                }
            }
        }

        return results;
    }

    #endregion

    #region Cache Key Helpers

    private static string GetUserCacheKey(Guid userId) => $"identity:user:{userId}";
    private static string GetUserCodeCacheKey(string userCode) => $"identity:user:code:{userCode}";
    private static string GetUserLookupCacheKey(List<Guid> userIds) => $"identity:user:lookup:{string.Join("-", userIds.OrderBy(x => x))}";
    private static string GetSellerInfoCacheKey(Guid userId) => $"identity:seller:{userId}";
    private static string GetSellerCodeCacheKey(string sellerCode) => $"identity:seller:code:{sellerCode}";
    private static string GetCompanyCacheKey(Guid companyId) => $"identity:company:{companyId}";
    private static string GetCompanyCodeCacheKey(string companyCode) => $"identity:company:code:{companyCode}";
    private static string GetEmailCacheKey(Guid userId) => $"identity:email:{userId}";
    private static string GetAddressCacheKey(Guid addressId) => $"identity:address:{addressId}";
    private static string GetAddressOwnerCacheKey(string ownerCode, UserTypeEnum userType) => $"identity:address:owner:{ownerCode}:{userType}";

    #endregion
}
