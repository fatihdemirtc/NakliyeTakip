using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace NakliyeTakip.MAUI.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthenticationService> _logger;
    private string? _accessToken;
    private string? _refreshToken;
    private string? _userId;
    private string? _userEmail;
    private string? _userName;

    // Keycloak configuration
    private const string KeycloakBaseUrl = "http://192.168.68.100:8080";
    private const string Realm = "NakliyeTenant";
    private const string ClientId = "mobile";
    private const string ClientSecret = "lCA00c0JnC0qiMt9nqk42Ely4DrZ63FM";

    public string? AccessToken => _accessToken;
    public string? UserId => _userId;
    public string? UserEmail => _userEmail;
    public string? UserName => _userName;
    public bool IsAuthenticated => !string.IsNullOrEmpty(_accessToken);

    public AuthenticationService(ILogger<AuthenticationService> logger)
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
        _logger = logger;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            var tokenEndpoint = $"{KeycloakBaseUrl}/realms/{Realm}/protocol/openid-connect/token";
            
            _logger.LogInformation("Attempting login to: {Endpoint}", tokenEndpoint);

            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("client_secret", ClientSecret),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("scope", "openid profile email")
            });

            var response = await _httpClient.PostAsync(tokenEndpoint, requestContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Login failed: {Error}", errorContent);
                return false;
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            
            if (tokenResponse?.AccessToken == null)
            {
                _logger.LogError("Token response is null or missing access token");
                return false;
            }

            _accessToken = tokenResponse.AccessToken;
            _refreshToken = tokenResponse.RefreshToken;

            // Parse JWT token to get user info
            ParseTokenClaims(_accessToken);

            _logger.LogInformation("Login successful for user: {Username}", username);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login to {BaseUrl}", KeycloakBaseUrl);
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            if (!string.IsNullOrEmpty(_refreshToken))
            {
                var logoutEndpoint = $"{KeycloakBaseUrl}/realms/{Realm}/protocol/openid-connect/logout";

                var requestContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", ClientId),
                    new KeyValuePair<string, string>("client_secret", ClientSecret),
                    new KeyValuePair<string, string>("refresh_token", _refreshToken)
                });

                await _httpClient.PostAsync(logoutEndpoint, requestContent);
            }

            ClearSession();
            _logger.LogInformation("User logged out");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            ClearSession();
        }
    }

    private void ParseTokenClaims(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            _userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            _userEmail = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            _userName = jwtToken.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;

            _logger.LogInformation("Token parsed - UserId: {UserId}, Email: {Email}", _userId, _userEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing token claims");
        }
    }

    private void ClearSession()
    {
        _accessToken = null;
        _refreshToken = null;
        _userId = null;
        _userEmail = null;
        _userName = null;
    }

    private class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }
    }
}
