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
    // ViewModel for managing and displaying the user's profile.
    public class ProfileViewModel : BaseViewModel
    {
        // Service for authentication and user management.
        private readonly AuthService _authService;

        // Backing fields for properties.
        private string _email;
        private string _firstName;
        private string _lastName;
        private string _profileImageBase64;
        private string _errorMessage;
        private bool _isEditing;

        // User email property.
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        // User first name.
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        // User last name.
        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        // Base64-encoded profile image string.
        public string ProfileImageBase64
        {
            get => _profileImageBase64;
            set => SetProperty(ref _profileImageBase64, value);
        }

        // Error message for displaying issues in the UI.
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        // Indicates whether the profile is in editing mode.
        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        // Command to switch to edit mode.
        public ICommand EditProfileCommand { get; }
        // Command to save profile changes.
        public ICommand SaveProfileCommand { get; }
        // Command to sign out the user.
        public ICommand SignOutCommand { get; }
        // Command to pick a new profile image.
        public ICommand PickProfileImageCommand { get; }

        // Constructor that initializes the ProfileViewModel with the AuthService.
        public ProfileViewModel(AuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));

            // Initialize commands.
            EditProfileCommand = new Command(ExecuteEditProfileCommand);
            SaveProfileCommand = new Command(async () => await ExecuteSaveProfileCommand());
            SignOutCommand = new Command(async () => await ExecuteSignOutCommand());
            PickProfileImageCommand = new Command(async () => await ExecutePickProfileImageCommand());

            // Load current user data.
            LoadUserData();
        }

        // Loads the user's data from the authentication service.
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

        // Switches the profile view into editing mode.
        private void ExecuteEditProfileCommand()
        {
            IsEditing = true;
        }

        // Allows the user to pick a new profile image and converts it to a Base64 string.
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
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    ProfileImageBase64 = Convert.ToBase64String(memoryStream.ToArray());
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to pick image: {ex.Message}";
            }
        }

        // Saves the changes to the user's profile.
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
                    // Update current user information.
                    var user = _authService.CurrentUser;
                    user.FirstName = FirstName;
                    user.LastName = LastName;
                    user.ProfileImageBase64 = ProfileImageBase64;

                    // Save changes via the authentication service.
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

        // Signs out the user and navigates to the login page.
        private async Task ExecuteSignOutCommand()
        {
            await _authService.SignOutAsync();
            await Shell.Current.GoToAsync("//login");
        }
    }
}
