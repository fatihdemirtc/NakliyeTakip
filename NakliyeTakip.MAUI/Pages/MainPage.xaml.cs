using NakliyeTakip.MAUI.Models;
using NakliyeTakip.MAUI.PageModels;

namespace NakliyeTakip.MAUI.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}