using IdentityModel.OidcClient;
using IdentityModel.Client;

namespace NakliyeTakip.MAUI.Services.Auth;

public class KeycloakAuthenticationService : IAuthenticationService
{
    private readonly OidcClient _oidcClient;
    private LoginResult? _currentLoginResult;

    public bool IsAuthenticated => _currentLoginResult?.AccessToken != null 
        && _currentLoginResult.ExpiresAt > DateTimeOffset.UtcNow;

    public KeycloakAuthenticationService()
    {
        var options = new OidcClientOptions
        {
            // Use mobile-public client (public + PKCE)
            Authority = "http://192.168.68.100:8080/realms/NakliyeTenant",
            ClientId = "mobile-public",
            Scope = "openid profile email",
            RedirectUri = "myapp://auth",
            PostLogoutRedirectUri = "myapp://auth",
            Browser = new WebAuthenticatorBrowser(),
            Policy = new Policy
            {
                // Allow HTTP for discovery in development
                Discovery = new DiscoveryPolicy
                {
                    RequireHttps = false
                }
            }
        };

        _oidcClient = new OidcClient(options);
    }

    public async Task<LoginResult> LoginAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("[KeycloakAuth] Starting login");
            var loginResult = await _oidcClient.LoginAsync(new LoginRequest());
            System.Diagnostics.Debug.WriteLine($"[KeycloakAuth] LoginResult IsError={loginResult.IsError} Error={loginResult.Error} Desc={loginResult.ErrorDescription}");
            System.Diagnostics.Debug.WriteLine($"[KeycloakAuth] AccessToken length={loginResult.AccessToken?.Length} RefreshToken length={loginResult.RefreshToken?.Length}");

            if (loginResult.IsError || string.IsNullOrEmpty(loginResult.AccessToken))
            {
                return new LoginResult
                {
                    IsSuccess = false,
                    ErrorMessage = loginResult.Error ?? loginResult.ErrorDescription ?? "Unknown login error"
                };
            }

            _currentLoginResult = new LoginResult
            {
                IsSuccess = true,
                AccessToken = loginResult.AccessToken,
                RefreshToken = loginResult.RefreshToken,
                ExpiresAt = loginResult.AccessTokenExpiration
            };

            await SecureStorage.SetAsync("access_token", loginResult.AccessToken);
            if (!string.IsNullOrEmpty(loginResult.RefreshToken))
            {
                await SecureStorage.SetAsync("refresh_token", loginResult.RefreshToken);
            }
            await SecureStorage.SetAsync("expires_at", loginResult.AccessTokenExpiration.ToString());

            System.Diagnostics.Debug.WriteLine($"[KeycloakAuth] Tokens stored. ExpiresAt={loginResult.AccessTokenExpiration}");
            return _currentLoginResult;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[KeycloakAuth] Exception during login: {ex}");
            return new LoginResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        if (IsAuthenticated)
        {
            return _currentLoginResult?.AccessToken;
        }

        var token = await SecureStorage.GetAsync("access_token");
        var expiresAtString = await SecureStorage.GetAsync("expires_at");
        System.Diagnostics.Debug.WriteLine($"[KeycloakAuth] Stored token exists={token!=null} expiresAtRaw={expiresAtString}");
        
        if (token != null && DateTimeOffset.TryParse(expiresAtString, out var expiresAt))
        {
            if (expiresAt > DateTimeOffset.UtcNow)
            {
                return token;
            }
            System.Diagnostics.Debug.WriteLine("[KeycloakAuth] Token expired, refreshing...");
            return await RefreshTokenAsync();
        }

        System.Diagnostics.Debug.WriteLine("[KeycloakAuth] No valid stored token");
        return null;
    }

    private async Task<string?> RefreshTokenAsync()
    {
        try
        {
            var refreshToken = await SecureStorage.GetAsync("refresh_token");
            System.Diagnostics.Debug.WriteLine($"[KeycloakAuth] Attempting refresh. RefreshToken present={!string.IsNullOrEmpty(refreshToken)}");
            if (string.IsNullOrEmpty(refreshToken))
                return null;

            var refreshResult = await _oidcClient.RefreshTokenAsync(refreshToken);
            System.Diagnostics.Debug.WriteLine($"[KeycloakAuth] RefreshResult IsError={refreshResult.IsError} Error={refreshResult.Error}");
            
            if (refreshResult.IsError || string.IsNullOrEmpty(refreshResult.AccessToken))
                return null;

            await SecureStorage.SetAsync("access_token", refreshResult.AccessToken);
            if (!string.IsNullOrEmpty(refreshResult.RefreshToken))
            {
                await SecureStorage.SetAsync("refresh_token", refreshResult.RefreshToken);
            }
            await SecureStorage.SetAsync("expires_at", refreshResult.AccessTokenExpiration.ToString());

            _currentLoginResult = new LoginResult
            {
                IsSuccess = true,
                AccessToken = refreshResult.AccessToken,
                RefreshToken = refreshResult.RefreshToken,
                ExpiresAt = refreshResult.AccessTokenExpiration
            };
            System.Diagnostics.Debug.WriteLine("[KeycloakAuth] Refresh successful");
            return refreshResult.AccessToken;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[KeycloakAuth] Refresh exception: {ex}");
            return null;
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("[KeycloakAuth] Starting logout");
            var logoutResult = await _oidcClient.LogoutAsync(new LogoutRequest());
            System.Diagnostics.Debug.WriteLine($"[KeycloakAuth] Logout IsError={logoutResult.IsError} Error={logoutResult.Error}");
            
            SecureStorage.Remove("access_token");
            SecureStorage.Remove("refresh_token");
            SecureStorage.Remove("expires_at");
            
            _currentLoginResult = null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[KeycloakAuth] Logout exception: {ex}");
        }
    }
}
