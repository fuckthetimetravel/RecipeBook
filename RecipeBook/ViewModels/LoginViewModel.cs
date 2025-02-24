using System.Threading.Tasks;
using RecipeBook.FirebaseConfig;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace RecipeBook.ViewModels;

public class LoginViewModel : BaseViewModel
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

    public Command LoginCommand { get; }
    public Command NavigateToRegisterCommand { get; }

    public LoginViewModel(FirestoreAuthService authService)
    {
        _authService = authService;
        LoginCommand = new Command(async () => await LoginAsync());
        NavigateToRegisterCommand = new Command(async () => await Shell.Current.GoToAsync("//register"));
    }

    private async Task LoginAsync()
    {
        try
        {
            var token = await _authService.LoginUserAsync(Email, Password);
            await SecureStorage.SetAsync("auth_token", token);
            await Shell.Current.GoToAsync("//profile");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
