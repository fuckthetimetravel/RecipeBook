using Microsoft.Maui.Controls;
using RecipeBook.ViewModels;

namespace RecipeBook.Views
{
    // Page for displaying the user's own recipes.
    public partial class MyRecipesPage : ContentPage
    {
        private readonly MyRecipesViewModel _viewModel;

        public MyRecipesPage(MyRecipesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
        }

        // Load recipes when the page appears.
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.LoadRecipesCommand?.Execute(null);
        }
    }
}
