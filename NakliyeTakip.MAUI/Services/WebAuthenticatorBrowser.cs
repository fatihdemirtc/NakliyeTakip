using IdentityModel.OidcClient.Browser;
using IdentityModel.Client;

namespace NakliyeTakip.MAUI.Services;

public class WebAuthenticatorBrowser : IdentityModel.OidcClient.Browser.IBrowser
{
    public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await WebAuthenticator.Default.AuthenticateAsync(
                new Uri(options.StartUrl),
                new Uri(options.EndUrl));

            var url = new RequestUrl(options.EndUrl)
                .Create(new Parameters(result.Properties));

            return new BrowserResult
            {
                Response = url,
                ResultType = BrowserResultType.Success
            };
        }
        catch (TaskCanceledException)
        {
            return new BrowserResult
            {
                ResultType = BrowserResultType.UserCancel,
                ErrorDescription = "Login was cancelled."
            };
        }
        catch (Exception ex)
        {
            return new BrowserResult
            {
                ResultType = BrowserResultType.UnknownError,
                Error = ex.ToString()
            };
        }
    }
}
