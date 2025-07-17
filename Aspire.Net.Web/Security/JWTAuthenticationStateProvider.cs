using Aspire.Net.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Aspire.Net.Web.Security
{
    public class JWTAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly AccessTokenService _accessTokenService;

        public JWTAuthenticationStateProvider(AccessTokenService accessTokenService)
        {
            _accessTokenService = accessTokenService;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _accessTokenService.GetToken();
                if (string.IsNullOrEmpty(token))
                    return await MarkAsUnauthenticated();

                var readJWT = new JwtSecurityTokenHandler().ReadJwtToken(token);
                var identity = new ClaimsIdentity(readJWT.Claims, "JWT");
                var principal = new ClaimsPrincipal(identity);

                return await Task.FromResult(new AuthenticationState(principal));

            }
            catch (Exception)
            {
                return await MarkAsUnauthenticated();
            }

        }
        public void NotifyAuthenticationStateChanged(AuthenticationState authenticationState)
        {
            NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
        }

        private async Task<AuthenticationState> MarkAsUnauthenticated()
        {
            try
            {
                var state = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                NotifyAuthenticationStateChanged(Task.FromResult(state));
                return state;
            }
            catch (Exception ex)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }
    }

}

