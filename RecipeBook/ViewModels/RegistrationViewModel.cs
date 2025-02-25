using Microsoft.Maui.Controls;
using RecipeBook.Services; // Подключаем новый AuthService
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RecipeBook.ViewModels
{
    public class RegistrationViewModel : BaseViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; } = DateTime.Today;
        public string Password { get; set; }
        public string RepeatPassword { get; set; }

        public ICommand RegisterCommand => new Command(async () => await RegisterAsync());
        public ICommand NavigateToLoginCommand => new Command(async () => await Shell.Current.GoToAsync("//login"));

        private async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await ShowAlert("Error", "Email and Password are required.");
                return;
            }

            if (Password != RepeatPassword)
            {
                await ShowAlert("Error", "Passwords do not match.");
                return;
            }

            try
            {
                // 📨 Передаём дату рождения при регистрации
                bool isRegistered = await AuthService.RegisterUser(
                    Email, Password, FirstName, LastName, BirthDate.ToString("yyyy-MM-dd"));

                if (isRegistered)
                {
                    await ShowAlert("Success", "Registration completed.");
                    await Shell.Current.GoToAsync("//login");
                }
                else
                {
                    await ShowAlert("Error", "Registration failed. Please try again.");
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
}
