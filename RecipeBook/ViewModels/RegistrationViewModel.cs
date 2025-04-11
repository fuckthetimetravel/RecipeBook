using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using RecipeBook.Services;

namespace RecipeBook.ViewModels
{
    // ViewModel that handles user registration logic.
    public class RegistrationViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private readonly RecipeService _recipeService;

        // Holds the selected profile image file.
        public FileResult SelectedProfileImage { get; set; }

        // Backing fields for user credentials and profile details.
        private string _email;
        private string _password;
        private string _confirmPassword;
        private string _firstName;
        private string _lastName;
        private string _profileImageBase64;
        private string _errorMessage;

        // User email.
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        // User password.
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        // Confirmation password.
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
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

        // Base64-encoded profile image.
        public string ProfileImageBase64
        {
            get => _profileImageBase64;
            set => SetProperty(ref _profileImageBase64, value);
        }

        // Error message for displaying validation or processing errors.
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        // Command to execute user registration.
        public ICommand SignUpCommand { get; }

        // Constructor that injects necessary services and initializes the sign-up command.
        public RegistrationViewModel(AuthService authService, RecipeService recipeService)
        {
            _authService = authService;
            _recipeService = recipeService;
            SignUpCommand = new Command(async () => await ExecuteSignUpCommand());
        }

        // Allows the user to pick a profile image from the device's gallery.
        public async Task PickProfileImageAsync()
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select a profile photo"
                });

                if (result != null)
                {
                    SelectedProfileImage = result;
                    // Converts the picked image to a Base64 string.
                    ProfileImageBase64 = await _recipeService.ConvertImageToBase64Async(result);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error picking image: {ex.Message}";
            }
        }

        // Allows the user to capture a profile photo using the device camera.
        public async Task TakeProfilePhotoAsync()
        {
            try
            {
                var result = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Take a profile photo"
                });

                if (result != null)
                {
                    SelectedProfileImage = result;
                    // Converts the captured image to a Base64 string.
                    ProfileImageBase64 = await _recipeService.ConvertImageToBase64Async(result);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error taking photo: {ex.Message}";
            }
        }

        // Validates input and registers the user.
        private async Task ExecuteSignUpCommand()
        {
            // Validate required fields.
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

            // Execute sign-up within a busy indicator.
            await ExecuteWithBusyIndicator(async () =>
            {
                try
                {
                    // Call the AuthService to register the user.
                    await _authService.SignUpAsync(
                        Email,
                        Password,
                        FirstName ?? string.Empty,
                        LastName ?? string.Empty,
                        ProfileImageBase64
                    );

                    // On successful sign-up, navigate to the main application shell.
                    Application.Current.MainPage = new AppShell();
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Sign up failed: {ex.Message}";
                }
            });
        }

        // Example method to navigate to the login page if needed.
        private async Task ExecuteGoToLoginCommand()
        {
            Application.Current.MainPage = new AppShell();
        }
    }
}
