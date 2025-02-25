using Microsoft.Maui.Controls;
using RecipeBook.ViewModels;
using RecipeBook.FirebaseConfig;

namespace RecipeBook.Views;

public partial class RegistrPage : ContentPage
{
    public RegistrPage()
    {
        InitializeComponent();
        BindingContext = new RegistrationViewModel();
    }
}
