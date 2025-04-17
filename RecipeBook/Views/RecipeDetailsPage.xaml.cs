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

        // Called when the page is about to disappear from view.
        // Invokes the ViewModel’s cleanup logic.
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _viewModel.OnDisappearing();
        }
    }
}
