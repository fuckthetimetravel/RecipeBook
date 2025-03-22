using Microsoft.Maui.Controls;
using RecipeBook.ViewModels;

namespace RecipeBook.Views
{
    public partial class MyRecipesPage : ContentPage
    {
        private readonly MyRecipesViewModel _viewModel;

        public MyRecipesPage(MyRecipesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Load recipes when page appears
            _viewModel.LoadRecipesCommand?.Execute(null);
        }
    }
}