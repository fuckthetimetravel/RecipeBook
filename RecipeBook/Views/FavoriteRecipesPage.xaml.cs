using Microsoft.Maui.Controls;
using RecipeBook.ViewModels;

namespace RecipeBook.Views
{
    // Page for displaying the user's favorite recipes.
    public partial class FavoriteRecipesPage : ContentPage
    {
        private readonly FavoriteRecipesViewModel _viewModel;

        public FavoriteRecipesPage(FavoriteRecipesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Load favorite recipes when the page appears.
            _viewModel.LoadFavoriteRecipesCommand?.Execute(null);
        }
    }
}
