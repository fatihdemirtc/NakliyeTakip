using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace NakliyeTakip.MAUI
{
    [Activity(Theme = "@style/Maui.SplashTheme",
         MainLauncher = true,
         LaunchMode = LaunchMode.SingleTop, // SingleTop kalmalı
         ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]

    public class MainActivity : MauiAppCompatActivity
    {
       
    }
}
