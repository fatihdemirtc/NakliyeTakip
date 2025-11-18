namespace NakliyeTakip.MAUI.Services;

public interface IAuthenticationService
{
    Task<LoginResult> LoginAsync();
    Task<string?> GetAccessTokenAsync();
    Task LogoutAsync();
    bool IsAuthenticated { get; }
}

public class LoginResult
{
    public bool IsSuccess { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
}
