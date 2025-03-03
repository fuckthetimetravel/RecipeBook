using Microsoft.Maui.Controls;
using RecipeBook.ViewModels;

namespace RecipeBook.Views
{
    public partial class SearchRecipesPage : ContentPage
    {
        public SearchRecipesPage()
        {
            InitializeComponent();
            BindingContext = new SearchRecipesViewModel();
        }
    }
}
