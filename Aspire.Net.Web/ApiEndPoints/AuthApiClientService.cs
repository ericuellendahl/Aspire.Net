using Aspire.Net.Web.DTOs.Auth;
using Aspire.Net.Web.Services;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Aspire.Net.Web.ApiEndPoints
{
    public class AuthApiClientService
    {
        private readonly AccessTokenService _accessTokenService;
        private readonly RefreshTokenService _refreshTokenService;
        private readonly NavigationManager _navigationManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;

        public AuthApiClientService(AccessTokenService accessTokenService, NavigationManager navigationManager, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, RefreshTokenService refreshTokenService)
        {
            _accessTokenService = accessTokenService;
            _navigationManager = navigationManager;
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _refreshTokenService = refreshTokenService;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("Auth/login", new { email, password });
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<AuthResponse>(token);
                await _accessTokenService.DeleteToken();
                await _accessTokenService.SetToken(result?.Token?.AccessToken);
                await _refreshTokenService.Set(result?.Token?.RefreshToken);

                return true;
            }
            return false;
        }

        public async Task<bool> RefreshTokenAsync()
        {
            var refreshToken = await _refreshTokenService.Get();
            if (string.IsNullOrEmpty(refreshToken))
            {
                return false;
            }
            _httpClient.DefaultRequestHeaders.Add("Cookie", $"refreshtoken={refreshToken}");
            var response = await _httpClient.PostAsync("Auth/refresh", null);
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    var result = JsonSerializer.Deserialize<AuthResponse>(token);
                    await _accessTokenService.SetToken(result?.Token?.AccessToken);
                    await _refreshTokenService.Set(result?.Token?.RefreshToken);
                    return true;
                }
            }
            return false;
        }

        public async Task Logout()
        {
            var accessToken = _httpContextAccessor.HttpContext?.Request.Cookies["access_token"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var refreshToken = await _refreshTokenService.Get();
            _httpClient.DefaultRequestHeaders.Add("Cookie", $"refreshtoken={refreshToken}");

            try
            {
                var response = await _httpClient.PostAsync("Auth/logout", null);
                if (response.IsSuccessStatusCode)
                {
                    await _accessTokenService.DeleteToken();
                    await _refreshTokenService.Delete();
                    _navigationManager.NavigateTo("/login", forceLoad: true);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
