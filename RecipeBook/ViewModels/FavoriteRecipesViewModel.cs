using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using RecipeBook.Models;
using RecipeBook.Services;

namespace RecipeBook.ViewModels
{
    // ViewModel for managing and displaying a user's favorite recipes.
    public class FavoriteRecipesViewModel : BaseViewModel
    {
        // Services used in the ViewModel.
        private readonly RecipeService _recipeService;
        private readonly AuthService _authService;

        // Collection holding the user's favorite recipes.
        private ObservableCollection<RecipeModel> _recipes;

        // Public property for data binding the list of favorite recipes.
        public ObservableCollection<RecipeModel> Recipes
        {
            get => _recipes;
            set => SetProperty(ref _recipes, value);
        }

        // Command to refresh the favorite recipes list.
        public ICommand RefreshCommand { get; }

        // Command triggered when a recipe is tapped for details.
        public ICommand RecipeTappedCommand { get; }

        // Command to remove a recipe from the favorites list.
        public ICommand RemoveFromFavoritesCommand { get; }

        // Command to load the favorite recipes.
        public ICommand LoadFavoriteRecipesCommand { get; }

        // Constructor that initializes the ViewModel with necessary services and commands.
        public FavoriteRecipesViewModel(RecipeService recipeService, AuthService authService)
        {
            _recipeService = recipeService;
            _authService = authService;
            Recipes = new ObservableCollection<RecipeModel>();

            RefreshCommand = new Command(async () => await LoadFavoriteRecipesAsync());
            RecipeTappedCommand = new Command<RecipeModel>(async (recipe) => await ExecuteRecipeTappedCommand(recipe));
            RemoveFromFavoritesCommand = new Command<RecipeModel>(async (recipe) => await ExecuteRemoveFromFavoritesCommand(recipe));
            LoadFavoriteRecipesCommand = new Command(async () => await LoadFavoriteRecipesAsync());
        }

        // Loads the user's favorite recipes asynchronously.
        public async Task LoadFavoriteRecipesAsync()
        {
            await ExecuteWithBusyIndicator(async () =>
            {
                // Ensure the user is signed in.
                if (!_authService.IsAuthenticated)
                {
                    ErrorMessage = "Please sign in to view your favorite recipes";
                    return;
                }

                // Retrieve favorite recipes from the RecipeService.
                var recipes = await _recipeService.GetFavoriteRecipesAsync();

                // Update the observable collection with the retrieved recipes.
                Recipes.Clear();
                foreach (var recipe in recipes)
                {
                    Recipes.Add(recipe);
                }
            });
        }

        // Executes when a recipe is tapped.
        private async Task ExecuteRecipeTappedCommand(RecipeModel recipe)
        {
            if (recipe != null)
            {
                // Navigate to the recipe details page.
                await Shell.Current.GoToAsync($"recipedetails?id={recipe.Id}");
            }
        }

        // Removes a recipe from the favorites list.
        private async Task ExecuteRemoveFromFavoritesCommand(RecipeModel recipe)
        {
            if (recipe != null)
            {
                await ExecuteWithBusyIndicator(async () =>
                {
                    // Remove the recipe from the favorite list via the RecipeService.
                    await _recipeService.RemoveFromFavoritesAsync(recipe.Id);
                    Recipes.Remove(recipe);
                });
            }
        }
    }
}
