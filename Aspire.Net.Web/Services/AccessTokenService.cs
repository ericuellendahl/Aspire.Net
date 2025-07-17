namespace Aspire.Net.Web.Services
{
    public class AccessTokenService
    {
        private readonly CookieService _cookieService;
        private readonly string tokenKey = "access_token";

        public AccessTokenService(CookieService cookieService)
        {
            _cookieService = cookieService;
        }

        public async Task SetToken(string accesstoken)
        => await _cookieService.SetCookieAsync(tokenKey, accesstoken, 1);

        public async Task<string?> GetToken()
        => await _cookieService.GetCookieAsync(tokenKey);

        public async Task DeleteToken()
        => await _cookieService.DeleteCookieAsync(tokenKey);
    }
}
