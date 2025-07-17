using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Aspire.Net.Web.Services;

public class RefreshTokenService(ProtectedLocalStorage protectedLocalStorage)
{
    private readonly ProtectedLocalStorage _protectedLocalStorage = protectedLocalStorage;
    private readonly string key = "refresh_token";

    public async Task Set(string value)
    {
        await _protectedLocalStorage.SetAsync(key, value);
    }

    public async Task<string?> Get()
    {
        var result = await _protectedLocalStorage.GetAsync<string>(key);
        return result.Success ? result.Value : null;
    }

    internal async Task Delete()
    {
        await _protectedLocalStorage.DeleteAsync(key);
    }
}
