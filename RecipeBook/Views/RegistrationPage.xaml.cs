using Microsoft.Maui.Controls;
using RecipeBook.ViewModels;
using RecipeBook.FirebaseConfig;

namespace RecipeBook.Views;

public partial class RegistrationPage : ContentPage
{
    public RegistrationPage()
    {
        InitializeComponent();
        BindingContext = new RegistrationViewModel(new FirestoreAuthService());
    }
}
