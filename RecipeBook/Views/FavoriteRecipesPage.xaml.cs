using Microsoft.Maui.Controls;
using RecipeBook.ViewModels;

namespace RecipeBook.Views
{
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
            // Load favorite recipes when page appears
            _viewModel.LoadFavoriteRecipesCommand?.Execute(null);
        }
    }
}