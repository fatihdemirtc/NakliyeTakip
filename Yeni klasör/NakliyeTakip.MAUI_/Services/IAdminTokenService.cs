namespace NakliyeTakip.MAUI.Services;

/// <summary>
/// Admin token service for managing service-to-service authentication.
/// </summary>
public interface IAdminTokenService
{
    /// <summary>
    /// Gets an admin access token using client credentials flow.
    /// </summary>
    Task<string?> GetAdminTokenAsync();
    
    /// <summary>
    /// Gets the current admin access token.
    /// </summary>
    string? AdminAccessToken { get; }
}