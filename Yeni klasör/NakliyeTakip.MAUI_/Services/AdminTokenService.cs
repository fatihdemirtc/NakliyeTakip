using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace NakliyeTakip.MAUI.Services;

public class AdminTokenService : IAdminTokenService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AdminTokenService> _logger;
    private string? _adminAccessToken;
    private DateTime _tokenExpiration;

    // Admin CLI configuration
    private const string KeycloakBaseUrl = "http://192.168.68.100:8080";
    private const string Realm = "NakliyeTenant";
    private const string AdminClientId = "admin-cli";
    private const string AdminClientSecret = "BcGQTSHVXNYSvmKi358kDVvV7QV2BHUv";

    public string? AdminAccessToken => _adminAccessToken;

    public AdminTokenService(ILogger<AdminTokenService> logger)
    {
        _httpClient = new HttpClient();
        _logger = logger;
    }

    public async Task<string?> GetAdminTokenAsync()
    {
        try
        {
            // Check if we have a valid token
            if (!string.IsNullOrEmpty(_adminAccessToken) && DateTime.UtcNow < _tokenExpiration)
            {
                _logger.LogInformation("Using cached admin token");
                return _adminAccessToken;
            }

            var tokenEndpoint = $"{KeycloakBaseUrl}/realms/{Realm}/protocol/openid-connect/token";

            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", AdminClientId),
                new KeyValuePair<string, string>("client_secret", AdminClientSecret)
            });

            var response = await _httpClient.PostAsync(tokenEndpoint, requestContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Admin token request failed: {Error}", errorContent);
                return null;
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();

            if (tokenResponse?.AccessToken == null)
            {
                _logger.LogError("Admin token response is null or missing access token");
                return null;
            }

            _adminAccessToken = tokenResponse.AccessToken;
            // Set expiration to 90% of actual expiration to refresh before it expires
            _tokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn * 0.9);

            _logger.LogInformation("Admin token acquired successfully, expires in {Seconds}s", tokenResponse.ExpiresIn);
            return _adminAccessToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting admin token");
            return null;
        }
    }

    private class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }
    }
}