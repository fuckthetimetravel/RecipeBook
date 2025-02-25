using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using RecipeBook.Services;

namespace RecipeBook.ViewModels;

public class ProfileViewModel : BaseViewModel
{
    private string _email;
    private string _firstName;
    private string _lastName;
    private string _birthDate;
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

    public string FirstName
    {
        get => _firstName;
        set
        {
            _firstName = value;
            OnPropertyChanged();
        }
    }

    public string LastName
    {
        get => _lastName;
        set
        {
            _lastName = value;
            OnPropertyChanged();
        }
    }

    public string BirthDate
    {
        get => _birthDate;
        set
        {
            _birthDate = value;
            OnPropertyChanged();
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

    public Command LogoutCommand { get; }

    public ProfileViewModel()
    {
        LogoutCommand = new Command(async () => await LogoutAsync());
        LoadUserProfile();
    }

    /// <summary>
    /// Загрузка данных профиля пользователя из AuthService
    /// </summary>
    private void LoadUserProfile()
    {
        try
        {
            if (!string.IsNullOrEmpty(AuthService.IdToken))
            {
                Email = AuthService.Email;
                FirstName = AuthService.FirstName;
                LastName = AuthService.LastName;
                BirthDate = AuthService.BirthDate;
                IsAuthenticated = true;
            }
            else
            {
                IsAuthenticated = false;
            }
        }
        catch (Exception ex)
        {
            ShowAlert("Error", ex.Message);
        }
    }

    /// <summary>
    /// Выход из аккаунта
    /// </summary>
    private async Task LogoutAsync()
    {
        try
        {
            SecureStorage.Remove("auth_token");
            AuthService.IdToken = null;
            AuthService.LocalId = null;

            Email = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            BirthDate = string.Empty;
            IsAuthenticated = false;

            await Shell.Current.GoToAsync("//login");
        }
        catch (Exception ex)
        {
            await ShowAlert("Error", ex.Message);
        }
    }

    /// <summary>
    /// Отображение сообщения об ошибке
    /// </summary>
    private static async Task ShowAlert(string title, string message)
    {
        await Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }
}
