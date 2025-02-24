using Microsoft.Maui.Controls;
using RecipeBook.ViewModels;
using RecipeBook.FirebaseConfig;

namespace RecipeBook.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = new LoginViewModel(new FirestoreAuthService());
    }
}
