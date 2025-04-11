using Microsoft.Maui.Controls;
using RecipeBook.ViewModels;

namespace RecipeBook.Views
{
    // RecipeDetailsPage binds to RecipeDetailsViewModel to display recipe details.
    public partial class RecipeDetailsPage : ContentPage
    {
        private readonly RecipeDetailsViewModel _viewModel;

        public RecipeDetailsPage(RecipeDetailsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
        }
    }
}
