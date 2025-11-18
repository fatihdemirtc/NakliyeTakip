namespace NakliyeTakip.MAUI
{
    public partial class App : Application
    {
        private readonly IAuthenticationService _authenticationService;

        public App(IAuthenticationService authenticationService)
        {
            InitializeComponent();
            _authenticationService = authenticationService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        protected override async void OnStart()
        {
            base.OnStart();

            // Kullanıcı giriş yapmamışsa login sayfasına yönlendir
            if (!_authenticationService.IsAuthenticated)
            {
                await Shell.Current.GoToAsync("///login");
            }
        }
    }
}