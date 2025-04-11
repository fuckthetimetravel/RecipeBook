using Microsoft.Maui.Controls;
using RecipeBook.ViewModels;

namespace RecipeBook.Views
{
    // LoginPage binds to LoginViewModel to handle user login.
    public partial class LoginPage : ContentPage
    {
        // Constructor that initializes the page and sets the BindingContext.
        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
