namespace NakliyeTakip.MAUI.Services;

/// <summary>
/// Authentication service for Keycloak token management.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Logs in the user with username and password.
    /// </summary>
    Task<bool> LoginAsync(string username, string password);

    /// <summary>
    /// Logs out the current user.
    /// </summary>
    Task LogoutAsync();

    /// <summary>
    /// Gets the current access token.
    /// </summary>
    string? AccessToken { get; }

    /// <summary>
    /// Gets the current user's ID.
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Gets the current user's email.
    /// </summary>
    string? UserEmail { get; }

    /// <summary>
    /// Gets the current user's name.
    /// </summary>
    string? UserName { get; }

    /// <summary>
    /// Checks if the user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }
}
