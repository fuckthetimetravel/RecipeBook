using System;
using System.Threading.Tasks;
using RecipeBook.FirebaseConfig;
using RecipeBook.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace RecipeBook.ViewModels;

public class ProfileViewModel : BaseViewModel
{
    private readonly FirestoreAuthService _authService;
    private string _email;
    private string _password;
    private User _currentUser;
    private bool _isAuthenticated;

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

    public User CurrentUser
    {
        get => _currentUser;
        set
        {
            _currentUser = value;
            IsAuthenticated = _currentUser != null;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsAuthenticated));
        }
    }

    public bool IsAuthenticated
    {
        get => _isAuthenticated;
        set
        {
            _isAuthenticated = value;
            OnPropertyChanged();
        }
    }

    public Command LoginCommand { get; }
    public Command RegisterCommand { get; }
    public Command LogoutCommand { get; }

    public ProfileViewModel(FirestoreAuthService authService)
    {
        _authService = authService;

        LoginCommand = new Command(async () => await LoginAsync());
        RegisterCommand = new Command(async () => await RegisterAsync());
        LogoutCommand = new Command(async () => await LogoutAsync());

        LoadUserProfile();
    }

    private async void LoadUserProfile()
    {
        try
        {
            var token = await SecureStorage.GetAsync("auth_token");
            if (!string.IsNullOrEmpty(token))
            {
                CurrentUser = await _authService.GetUserProfileAsync(token);
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async Task LoginAsync()
    {
        try
        {
            var token = await _authService.LoginUserAsync(Email, Password);
            await SecureStorage.SetAsync("auth_token", token);
            LoadUserProfile();
            await Application.Current.MainPage.DisplayAlert("Success", "Login successful!", "OK");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async Task RegisterAsync()
    {
        try
        {
            var token = await _authService.RegisterUserAsync(Email, Password);
            await SecureStorage.SetAsync("auth_token", token);
            LoadUserProfile();
            await Application.Current.MainPage.DisplayAlert("Success", "Registration successful!", "OK");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async Task LogoutAsync()
    {
        try
        {
            await _authService.LogoutAsync();
            CurrentUser = null;
            Email = string.Empty;
            Password = string.Empty;

            // Перенаправляем пользователя на страницу входа
            await Shell.Current.GoToAsync("//login");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }


}
