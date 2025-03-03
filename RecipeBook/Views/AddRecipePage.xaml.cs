using Microsoft.Maui.Controls;
using RecipeBook.ViewModels;

namespace RecipeBook.Views
{
    public partial class AddRecipePage : ContentPage
    {
        public AddRecipePage()
        {
            InitializeComponent();
            BindingContext = new AddRecipeViewModel();
        }
    }
}
