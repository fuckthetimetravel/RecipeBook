using Microsoft.Maui.Controls;
using RecipeBook.ViewModels;

namespace RecipeBook.Views
{
    // AddRecipePage binds to AddRecipeViewModel and serves as the UI for adding a new recipe.
    public partial class AddRecipePage : ContentPage
    {
        // Constructor that initializes the page and sets its BindingContext.
        public AddRecipePage(AddRecipeViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
