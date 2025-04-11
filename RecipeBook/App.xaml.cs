using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Extensions.DependencyInjection;
using RecipeBook.Services;
using RecipeBook.ViewModels;
using RecipeBook.Views;

namespace RecipeBook
{
    // Main application class for RecipeBook.
    public partial class App : Application
    {
        // Constructor receives required services and view models via dependency injection.
        public App(AuthService authService, LoginViewModel loginViewModel)
        {
            InitializeComponent();

            // Placeholder to avoid exceptions during initialization.
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

        // Asynchronously initializes the app by loading saved auth data.
        private async void InitializeApp(AuthService authService, LoginViewModel loginViewModel)
        {
            // Load saved authentication data
            await authService.LoadSavedAuthDataAsync();

            // If the user is authenticated, navigate to the main shell; otherwise, show the login page.
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
