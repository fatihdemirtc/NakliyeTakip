using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace NakliyeTakip.MAUI.PageModels;

public partial class LoginPageModel(IAuthenticationService _authenticationService) : ObservableObject
{
    private readonly IAuthenticationService _authenticationService;   

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Kullanýcý adý ve þifre gereklidir";
            return;
        }

        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var success = await _authenticationService.LoginAsync(Username, Password);

            if (success)
            {                
                // Ana sayfaya yönlendir
                await Shell.Current.GoToAsync("///main");
            }
            else
            {
                ErrorMessage = "Kullanýcý adý veya þifre hatalý";                
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Giriþ yapýlýrken bir hata oluþtu";            
        }
        finally
        {
            IsLoading = false;
        }
    }
}
