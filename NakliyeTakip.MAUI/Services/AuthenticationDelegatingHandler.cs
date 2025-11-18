namespace NakliyeTakip.MAUI.Services;

public class AuthenticationDelegatingHandler : DelegatingHandler
{
    private readonly IAuthenticationService _authService;

    public AuthenticationDelegatingHandler(IAuthenticationService authService)
    {
        _authService = authService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        var token = await _authService.GetAccessTokenAsync();
        
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
