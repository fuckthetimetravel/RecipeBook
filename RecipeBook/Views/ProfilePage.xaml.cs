using Microsoft.Maui.Controls;
using RecipeBook.ViewModels;
using RecipeBook.FirebaseConfig;

namespace RecipeBook.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
        BindingContext = new ProfileViewModel();
    }
}
