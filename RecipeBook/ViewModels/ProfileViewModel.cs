using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel;
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
        private string _profileImageBase64;
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

        public string ProfileImageBase64
        {
            get => _profileImageBase64;
            set => SetProperty(ref _profileImageBase64, value);
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
        public ICommand PickProfileImageCommand { get; }

        public ProfileViewModel(AuthService authService)
        {
            _authService = authService;

            EditProfileCommand = new Command(ExecuteEditProfileCommand);
            SaveProfileCommand = new Command(async () => await ExecuteSaveProfileCommand());
            SignOutCommand = new Command(async () => await ExecuteSignOutCommand());
            PickProfileImageCommand = new Command(async () => await ExecutePickProfileImageCommand());

            LoadUserData();
        }

        public void LoadUserData()
        {
            if (_authService.IsAuthenticated && _authService.CurrentUser != null)
            {
                Email = _authService.CurrentUser.Email;
                FirstName = _authService.CurrentUser.FirstName ?? string.Empty;
                LastName = _authService.CurrentUser.LastName ?? string.Empty;
                ProfileImageBase64 = _authService.CurrentUser.ProfileImageBase64 ?? string.Empty;
            }
        }

        private void ExecuteEditProfileCommand()
        {
            IsEditing = true;
        }

        private async Task ExecutePickProfileImageCommand()
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Pick a profile photo"
                });

                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                    using var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    ProfileImageBase64 = Convert.ToBase64String(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to pick image: {ex.Message}";
            }
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
                    var user = _authService.CurrentUser;
                    user.FirstName = FirstName;
                    user.LastName = LastName;
                    user.ProfileImageBase64 = ProfileImageBase64;

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
