using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using RecipeBook.Services;
using RecipeBook.Views;
using Microsoft.Extensions.DependencyInjection;

namespace RecipeBook.ViewModels
{
    // ViewModel for handling user login functionality.
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private string _email;
        private string _password;

        // User email for login.
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        // User password for login.
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        // Command that triggers the login process.
        public ICommand LoginCommand { get; }
        // Command that navigates to the registration page.
        public ICommand RegisterCommand { get; }

        // Constructor that initializes the LoginViewModel with required services.
        public LoginViewModel(AuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            Title = "Login";

            // Initialize commands with asynchronous execution.
            LoginCommand = new Command(async () => await ExecuteLoginCommand());
            RegisterCommand = new Command(async () =>
            {
                // Retrieve the RegistrationViewModel via dependency injection.
                var registrationViewModel = App.Current.Handler.MauiContext.Services.GetService<RegistrationViewModel>();
                if (registrationViewModel == null)
                    throw new InvalidOperationException("Unable to retrieve RegistrationViewModel.");

                // Create the registration page using the obtained ViewModel.
                var registrationPage = new RegistrPage(registrationViewModel);

                // Navigate to the registration page.
                await Application.Current.MainPage.Navigation.PushAsync(registrationPage);
            });
        }

        // Executes the login process with busy indicator and error handling.
        private async Task ExecuteLoginCommand()
        {
            await ExecuteWithBusyIndicator(async () =>
            {
                // Validate that both email and password are provided.
                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Email and password are required";
                    return;
                }

                // Attempt user authentication.
                await _authService.SignInAsync(Email, Password);

                // On successful login, set the main page to the application's main shell.
                Application.Current.MainPage = new AppShell();
            });
        }
    }
}
