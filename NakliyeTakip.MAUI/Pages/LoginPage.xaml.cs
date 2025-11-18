using NakliyeTakip.MAUI.Services;

namespace NakliyeTakip.MAUI.Pages;

public partial class LoginPage : ContentPage
{
    private readonly IAuthenticationService _authService;

    public LoginPage(IAuthenticationService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        // Eðer zaten login olmuþsa ana sayfaya yönlendir
        if (_authService.IsAuthenticated)
        {
            await Shell.Current.GoToAsync("///main");
        }
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        try
        {
            // UI'yý güncelle - Loading baþlat
            LoginButton.IsEnabled = false;
            LoginButton.Text = "Lütfen bekleyin...";
            LoadingIndicator.IsRunning = true;
            LoadingIndicator.IsVisible = true;
            LoadingLabel.IsVisible = true;

            var result = await _authService.LoginAsync();

            if (result.IsSuccess)
            {
                LoadingLabel.Text = "Baþarýlý! Yönlendiriliyor...";
                await Task.Delay(500); // Kullanýcýya feedback göster
                
                // Ana sayfaya yönlendir
                await Shell.Current.GoToAsync("///main");
            }
            else
            {
                await DisplayAlert("Giriþ Baþarýsýz", 
                    $"Giriþ yapýlamadý.\n\nHata: {result.ErrorMessage}", 
                    "Tamam");
            }
        }
        catch (TaskCanceledException)
        {
            await DisplayAlert("Ýptal Edildi", 
                "Giriþ iþlemi iptal edildi.", 
                "Tamam");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", 
                $"Beklenmeyen bir hata oluþtu:\n\n{ex.Message}", 
                "Tamam");
        }
        finally
        {
            // UI'yý eski haline getir
            LoginButton.IsEnabled = true;
            LoginButton.Text = "Keycloak ile Giriþ Yap";
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
            LoadingLabel.IsVisible = false;
            LoadingLabel.Text = "Giriþ yapýlýyor...";
        }
    }
}
