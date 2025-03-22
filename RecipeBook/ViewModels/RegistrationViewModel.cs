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

        public ICommand RegisterCommand { get; }
        public ICommand LoginCommand { get; }

        public RegistrationViewModel(AuthService authService)
        {
            _authService = authService;
            Title = "Register";

            RegisterCommand = new Command(async () => await ExecuteRegisterCommand());
            LoginCommand = new Command(async () => await Shell.Current.GoToAsync("/login"));
        }

        private async Task ExecuteRegisterCommand()
        {
            await ExecuteWithBusyIndicator(async () =>
            {
                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
                {
                    ErrorMessage = "All fields are required";
                    return;
                }

                if (Password != ConfirmPassword)
                {
                    ErrorMessage = "Passwords do not match";
                    return;
                }

                await _authService.SignUpAsync(Email, Password);
                await Shell.Current.GoToAsync("/profile");
            });
        }
    }
}