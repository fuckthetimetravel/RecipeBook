using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using RecipeBook.Services;

namespace RecipeBook.ViewModels
{
    public class RegistrationViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        private string _email;
        private string _password;
        private string _confirmPassword;
        private string _firstName;
        private string _lastName;
        private string _errorMessage;

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand SignUpCommand { get; }

        public RegistrationViewModel(AuthService authService)
        {
            _authService = authService;

            SignUpCommand = new Command(async () => await ExecuteSignUpCommand());
        }

        private async Task ExecuteSignUpCommand()
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Email is required";
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Password is required";
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match";
                return;
            }

            await ExecuteWithBusyIndicator(async () =>
            {
                try
                {
                    // Sign up user
                    await _authService.SignUpAsync(Email, Password, FirstName ?? string.Empty, LastName ?? string.Empty);

                    // Navigate to main page
                    await Shell.Current.GoToAsync("//profile");
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Sign up failed: {ex.Message}";
                }
            });
        }

        private async Task ExecuteGoToLoginCommand()
        {
            Application.Current.MainPage = new AppShell();
        }
    }
}