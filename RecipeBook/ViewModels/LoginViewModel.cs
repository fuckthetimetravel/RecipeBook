using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using RecipeBook.Services; // Используем новый AuthService

namespace RecipeBook.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private string _email;
    private string _password;

    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged();
        }
    }

    public Command LoginCommand { get; }
    public Command NavigateToRegisterCommand { get; }

    public LoginViewModel()
    {
        LoginCommand = new Command(async () => await LoginAsync());
        NavigateToRegisterCommand = new Command(async () => await Shell.Current.GoToAsync("//register"));
    }

    /// <summary>
    /// Авторизация пользователя через AuthService
    /// </summary>
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await ShowAlert("Error", "Email and Password are required.");
            return;
        }

        try
        {
            bool isLoggedIn = await AuthService.LoginUser(Email, Password);

            if (isLoggedIn)
            {
                await SecureStorage.SetAsync("auth_token", AuthService.IdToken);
                await ShowAlert("Success", "Login successful!");
                await Shell.Current.GoToAsync("//profile");
            }
            else
            {
                await ShowAlert("Error", "Invalid email or password.");
            }
        }
        catch (Exception ex)
        {
            await ShowAlert("Error", ex.Message);
        }
    }

    /// <summary>
    /// Утилитарный метод для отображения сообщений
    /// </summary>
    private static async Task ShowAlert(string title, string message)
    {
        await Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }
}
