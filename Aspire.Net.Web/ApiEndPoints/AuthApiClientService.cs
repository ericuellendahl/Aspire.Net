using Aspire.Net.Web.DTOs.Auth;
using Aspire.Net.Web.Services;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Aspire.Net.Web.ApiEndPoints
{
    public class AuthApiClientService(AccessTokenService accessTokenService,
                                NavigationManager navigationManager,
                                IHttpClientFactory httpClientFactory,
                                RefreshTokenService refreshTokenService)
    {
        private readonly AccessTokenService _accessTokenService = accessTokenService;
        private readonly RefreshTokenService _refreshTokenService = refreshTokenService;
        private readonly NavigationManager _navigationManager = navigationManager;
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");

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
            var accessToken = await _accessTokenService.GetToken();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var refreshToken = await _refreshTokenService.Get();
            _httpClient.DefaultRequestHeaders.Add("Cookie", $"refreshtoken={refreshToken}");

            await _httpClient.PostAsync("Auth/logout", null);
            await _accessTokenService.DeleteToken();
            await _refreshTokenService.Delete();

            _navigationManager.NavigateTo("/login", forceLoad: true);
        }
    }
}
