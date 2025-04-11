using Microsoft.Maui.Controls;
using RecipeBook.Models;
using RecipeBook.ViewModels;
using System.Threading.Tasks;

namespace RecipeBook.Views
{
    // SearchRecipesPage is responsible for displaying and filtering recipes
    // based on a search query and ingredient filters.
    public partial class SearchRecipesPage : ContentPage
    {
        private readonly SearchRecipesViewModel _viewModel;

        // Constructor accepts a SearchRecipesViewModel instance via dependency injection.
        public SearchRecipesPage(SearchRecipesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
        }

        // When the page appears, load the complete list of recipes.
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.LoadRecipesCommand?.Execute(null);
        }

        // Handler for the "Filter" button click.
        // It prompts the user for an ingredient name and optional quantity,
        // then adds the ingredient to the ViewModel and applies the filter.
        private async void OnFilterButtonClicked(object sender, System.EventArgs e)
        {
            // Prompt the user to enter an ingredient name.
            string ingredientName = await DisplayPromptAsync("Add Ingredient", "Enter ingredient name:");

            // Proceed only if a valid ingredient name is provided.
            if (!string.IsNullOrWhiteSpace(ingredientName))
            {
                // Prompt for an optional quantity.
                string quantity = await DisplayPromptAsync("Add Quantity", "Enter quantity (optional):", initialValue: "");

                // Assign the entered ingredient name and quantity to the ViewModel properties.
                _viewModel.NewIngredientName = ingredientName;

                // Execute the command to add the ingredient to the list.
                _viewModel.AddIngredientCommand.Execute(null);

                // Ask the user if they wish to add another ingredient.
                bool addMore = await DisplayAlert("Add More?", "Do you want to add more ingredients?", "Yes", "No");
                if (addMore)
                {
                    // Recursively invoke the same method to add another ingredient.
                    OnFilterButtonClicked(sender, e);
                }
                else
                {
                    // Once finished, apply the filter on the recipes by the added ingredients.
                    _viewModel.FilterByIngredientsCommand.Execute(null);
                }
            }
        }

        // Handler for when a recipe is selected from the list.
        // Navigates to the recipe details page using the recipe's ID.
        private void OnRecipeSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is RecipeModel recipe)
            {
                // Navigate asynchronously to the RecipeDetailsPage.
                Shell.Current.GoToAsync($"recipedetails?id={recipe.Id}");
            }
        }
    }
}
