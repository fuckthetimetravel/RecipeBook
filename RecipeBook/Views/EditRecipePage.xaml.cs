using Microsoft.Maui.Controls;
using RecipeBook.ViewModels;

namespace RecipeBook.Views
{
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