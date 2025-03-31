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

        private string _email;
        private string _firstName;
        private string _lastName;
        private string _errorMessage;
        private bool _isEditing;

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
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

        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        public ICommand EditProfileCommand { get; }
        public ICommand SaveProfileCommand { get; }
        public ICommand SignOutCommand { get; }

        public ProfileViewModel(AuthService authService)
        {
            _authService = authService;

            EditProfileCommand = new Command(ExecuteEditProfileCommand);
            SaveProfileCommand = new Command(async () => await ExecuteSaveProfileCommand());
            SignOutCommand = new Command(async () => await ExecuteSignOutCommand());

            // Load user data
            LoadUserData();
        }

        public void LoadUserData()
        {
            if (_authService.IsAuthenticated && _authService.CurrentUser != null)
            {
                Email = _authService.CurrentUser.Email;
                FirstName = _authService.CurrentUser.FirstName ?? string.Empty;
                LastName = _authService.CurrentUser.LastName ?? string.Empty;
            }
        }

        private void ExecuteEditProfileCommand()
        {
            IsEditing = true;
        }

        private async Task ExecuteSaveProfileCommand()
        {
            if (!_authService.IsAuthenticated)
            {
                ErrorMessage = "You must be logged in to update your profile";
                return;
            }

            await ExecuteWithBusyIndicator(async () =>
            {
                try
                {
                    // Update user data
                    var user = _authService.CurrentUser;
                    user.FirstName = FirstName;
                    user.LastName = LastName;

                    // Save to database
                    await _authService.UpdateUserAsync(user);

                    IsEditing = false;
                    ErrorMessage = string.Empty;
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Failed to update profile: {ex.Message}";
                }
            });
        }

        private async Task ExecuteSignOutCommand()
        {
            await _authService.SignOutAsync();
            await Shell.Current.GoToAsync("//login");
        }
    }
}