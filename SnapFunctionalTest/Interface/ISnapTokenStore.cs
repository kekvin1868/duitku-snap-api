using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Refit;
using SnapFunctionalTest.Contracts;
using SnapFunctionalTest.Helpers;

namespace SnapFunctionalTest.Interface;

public interface ISnapTokenStore
{
    Task<string?> GetToken();
}

public class SnapTokenStore : ISnapTokenStore
{
    private readonly IMemoryCache _memoryCache;
    private readonly ISnapTokenApi _tokenApi;
    private const string CacheKey = "SNAP-TOKEN";
    private readonly SnapSettings _settings;

    public SnapTokenStore(IMemoryCache memoryCache, ISnapTokenApi tokenApi, IOptions<SnapSettings> setting)
    {
        _memoryCache = memoryCache;
        _tokenApi = tokenApi;
        _settings = setting.Value;
    }

    public async Task<string?> GetToken()
    {
        try
        {
            if (!_memoryCache.TryGetValue(CacheKey, out string? value))
            {
                string timestamp = DateTime.Now.ToString("O");
                string stringToSign = $"{_settings.ClientId}|{timestamp}";
                string signature = CryptoHelper.SHA256WithRSA(stringToSign, _settings.PrivateKey);
                
                var tokenResponse = await _tokenApi.GetToken(
                    new SnapTokenRequest(),
                    timestamp,
                    _settings.ClientId,
                    signature);
                await tokenResponse.EnsureSuccessStatusCodeAsync();
                value = tokenResponse.Content!.AccessToken;
                
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(14));

                _memoryCache.Set(CacheKey, value, cacheEntryOptions);
            }

            return value;
        }
        catch (ApiException e)
        {
            Console.WriteLine("Something went wrong when calling api token.");
            throw;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}