using Android.App;
using Android.Content;
using Android.Content.PM;

namespace NakliyeTakip.MAUI;

// Bu aktivite sadece yönlendirmeyi yakalamak için var.
// NoHistory = true sayesinde işi bitince geçmişten silinir.
[Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
[IntentFilter(new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataScheme = "myapp",  // Redirect URI'deki şema
    DataHost = "auth")]    // Redirect URI'deki host
public class WebAuthenticationCallbackActivity : WebAuthenticatorCallbackActivity
{
    // İçini boş bırakın, miras aldığı sınıf (WebAuthenticatorCallbackActivity) işi yapacak.
}