using Microsoft.Maui.Controls;
using RecipeBook.Views;
using Microsoft.Maui.Storage;

namespace RecipeBook;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        //CheckAuthentication();

        Routing.RegisterRoute("profile", typeof(ProfilePage));
        Routing.RegisterRoute("login", typeof(LoginPage));
        Routing.RegisterRoute("register", typeof(RegistrPage));
        Routing.RegisterRoute("addRecipe", typeof(AddRecipePage));
        Routing.RegisterRoute("search", typeof(SearchRecipesPage));



    }

    /// <summary>
    /// Проверяем, вошел ли пользователь в систему
    /// </summary>
    private async void CheckAuthentication()
    {
        var token = await SecureStorage.GetAsync("auth_token");

        if (string.IsNullOrEmpty(token))
        {
            await Shell.Current.GoToAsync("//login");
        }

    }
}
