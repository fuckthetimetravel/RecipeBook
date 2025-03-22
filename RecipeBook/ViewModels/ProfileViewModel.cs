using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using RecipeBook.Models;
using RecipeBook.Services;

namespace RecipeBook.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private User _user;
        private bool _isAuthenticated;

        public User User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            set => SetProperty(ref _isAuthenticated, value);
        }

        public ICommand LogoutCommand { get; }
        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        public ProfileViewModel(AuthService authService)
        {
            _authService = authService;
            Title = "Profile";

            LogoutCommand = new Command(ExecuteLogoutCommand);
            LoginCommand = new Command(async () => await Shell.Current.GoToAsync("/login"));
            RegisterCommand = new Command(async () => await Shell.Current.GoToAsync("/register"));

            LoadUser();
        }

        public void LoadUser()
        {
            User = _authService.CurrentUser;
            IsAuthenticated = _authService.IsAuthenticated;
        }

        private void ExecuteLogoutCommand()
        {
            _authService.SignOut();
            LoadUser();
        }
    }
}