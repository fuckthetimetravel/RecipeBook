using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Extensions.DependencyInjection;
using RecipeBook.Services;
using RecipeBook.ViewModels;
using RecipeBook.Views;

namespace RecipeBook
{
    public partial class App : Application
    {
        public App(AuthService authService, LoginViewModel loginViewModel)
        {
            InitializeComponent();

            // Временная заглушка, чтобы избежать исключения
            MainPage = new ContentPage
            {
                Content = new ActivityIndicator
                {
                    IsRunning = true,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                }
            };

            InitializeApp(authService, loginViewModel);
        }


        private async void InitializeApp(AuthService authService, LoginViewModel loginViewModel)
        {
            await authService.LoadSavedAuthDataAsync();

            if (authService.IsAuthenticated)
            {
                MainPage = new AppShell();
            }
            else
            {
                MainPage = new NavigationPage(new LoginPage(loginViewModel));
            }
        }
    }

}
