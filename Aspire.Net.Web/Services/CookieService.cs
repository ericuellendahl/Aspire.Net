using Microsoft.JSInterop;

namespace Aspire.Net.Web.Services
{
    public class CookieService(IJSRuntime jsRuntime)
    {
        private readonly IJSRuntime _jsRuntime = jsRuntime;

        public async Task SetCookieAsync(string key, string value, int days)
         => await _jsRuntime.InvokeVoidAsync("setCookie", key, value, days);

        public async Task<string?> GetCookieAsync(string key)
        => await _jsRuntime.InvokeAsync<string>("getCookie", key);

        public async Task DeleteCookieAsync(string key)
        => await _jsRuntime.InvokeVoidAsync("deleteCookie", key);
    }
}
