using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using RecipeBook.Services;
using RecipeBook.Views;

namespace RecipeBook.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private string _email;
        private string _password;

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

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
            Title = "Login";

            LoginCommand = new Command(async () => await ExecuteLoginCommand());
            RegisterCommand = new Command(async () =>
            {
                var registrationViewModel = App.Current.Handler.MauiContext.Services.GetService<RegistrationViewModel>();
                var registrationPage = new RegistrPage(registrationViewModel);

                await Application.Current.MainPage.Navigation.PushAsync(registrationPage);
            });

        }

        private async Task ExecuteLoginCommand()
        {
            await ExecuteWithBusyIndicator(async () =>
            {
                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Email and password are required";
                    return;
                }

                await _authService.SignInAsync(Email, Password);

                Application.Current.MainPage = new AppShell();
            });
        }
    }
}