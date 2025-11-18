using NakliyeTakip.MAUI.Services;

namespace NakliyeTakip.MAUI.Pages;

public partial class HomePage : ContentPage
{
    private readonly IAuthenticationService _authService;
    private string? _fullToken;

    public HomePage(IAuthenticationService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadTokenInfoAsync();
    }

    private async Task LoadTokenInfoAsync()
    {
        var token = await _authService.GetAccessTokenAsync();
        
        if (!string.IsNullOrEmpty(token))
        {
            _fullToken = token;
            
            // Token'ýn ilk ve son 20 karakterini göster
            var displayToken = token.Length > 40 
                ? $"{token[..20]}...{token[^20..]}" 
                : token;
            
            TokenLabel.Text = displayToken;
        }
        else
        {
            _fullToken = null;
            TokenLabel.Text = "Token bulunamadý";
        }
    }

    private async void OnCopyTokenClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(_fullToken))
        {
            await Clipboard.SetTextAsync(_fullToken);
            await DisplayAlert("Baþarýlý", "Token panoya kopyalandý!", "Tamam");
        }
        else
        {
            await DisplayAlert("Hata", "Kopyalanacak token bulunamadý", "Tamam");
        }
    }

    private async void OnRefreshTokenClicked(object sender, EventArgs e)
    {
        await LoadTokenInfoAsync();
        await DisplayAlert("Bilgi", "Token bilgileri yenilendi", "Tamam");
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        var confirm = await DisplayAlert(
            "Güvenli Çýkýþ", 
            "Oturumunuzu kapatmak istediðinizden emin misiniz?", 
            "Evet, Çýkýþ Yap", 
            "Ýptal");

        if (confirm)
        {
            await _authService.LogoutAsync();
            await Shell.Current.GoToAsync("///login");
        }
    }
}
