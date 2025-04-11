using Microsoft.Maui.Controls;
using RecipeBook.ViewModels;

namespace RecipeBook.Views
{
    // Page for editing a recipe that binds to EditRecipeViewModel.
    public partial class EditRecipePage : ContentPage
    {
        private readonly EditRecipeViewModel _viewModel;

        public EditRecipePage(EditRecipeViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
        }
    }
}
