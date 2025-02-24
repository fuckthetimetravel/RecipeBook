using System.Threading.Tasks;
using RecipeBook.FirebaseConfig;
using Microsoft.Maui.Controls;

namespace RecipeBook.ViewModels;

public class RegistrationViewModel : BaseViewModel
{
    private readonly FirestoreAuthService _authService;
    private string _email;
    private string _password;

    public string Email
    {
        get => _email;
        set { _email = value; OnPropertyChanged(); }
    }

    public string Password
    {
        get => _password;
        set { _password = value; OnPropertyChanged(); }
    }

    public Command RegisterCommand { get; }
    public Command NavigateToLoginCommand { get; }

    public RegistrationViewModel(FirestoreAuthService authService)
    {
        _authService = authService;
        RegisterCommand = new Command(async () => await RegisterAsync());
        NavigateToLoginCommand = new Command(async () => await Shell.Current.GoToAsync("//login"));
    }

    private async Task RegisterAsync()
    {
        try
        {
            var token = await _authService.RegisterUserAsync(Email, Password);
            await SecureStorage.SetAsync("auth_token", token);
            await Shell.Current.GoToAsync("//profile");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
