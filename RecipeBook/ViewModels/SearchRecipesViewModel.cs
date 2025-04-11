using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using RecipeBook.Models;
using RecipeBook.Services;

namespace RecipeBook.ViewModels
{
    // ViewModel for searching recipes by title and filtering by ingredients.
    public class SearchRecipesViewModel : BaseViewModel
    {
        private readonly RecipeService _recipeService;
        private string _searchQuery;
        private ObservableCollection<RecipeModel> _searchResults;
        private ObservableCollection<Ingredient> _ingredients;
        private string _newIngredientName;

        // User-entered text to search for recipes.
        public string SearchQuery
        {
            get => _searchQuery;
            set => SetProperty(ref _searchQuery, value);
        }

        // Collection of recipes matching the search criteria.
        public ObservableCollection<RecipeModel> SearchResults
        {
            get => _searchResults;
            set => SetProperty(ref _searchResults, value);
        }

        // Collection of ingredients to filter recipes.
        public ObservableCollection<Ingredient> Ingredients
        {
            get => _ingredients;
            set => SetProperty(ref _ingredients, value);
        }

        // Name of the ingredient to add to the ingredients filter list.
        public string NewIngredientName
        {
            get => _newIngredientName;
            set => SetProperty(ref _newIngredientName, value);
        }

        // Command to execute a search based on the search query.
        public ICommand SearchCommand { get; }
        // Command triggered when a recipe is tapped.
        public ICommand RecipeTappedCommand { get; }
        // Command to add an ingredient to the filter list.
        public ICommand AddIngredientCommand { get; }
        // Command to remove an ingredient from the filter list.
        public ICommand RemoveIngredientCommand { get; }
        // Command to filter recipes based on the selected ingredients.
        public ICommand FilterByIngredientsCommand { get; }
        // Command to load all recipes.
        public ICommand LoadRecipesCommand { get; }

        // Constructor initializes collections and commands.
        public SearchRecipesViewModel(RecipeService recipeService)
        {
            _recipeService = recipeService;
            SearchResults = new ObservableCollection<RecipeModel>();
            Ingredients = new ObservableCollection<Ingredient>();

            // Initialize commands with their respective handlers.
            SearchCommand = new Command(async () => await ExecuteSearchCommand());
            RecipeTappedCommand = new Command<RecipeModel>(async (recipe) => await ExecuteRecipeTappedCommand(recipe));
            AddIngredientCommand = new Command(ExecuteAddIngredientCommand);
            RemoveIngredientCommand = new Command<Ingredient>(ExecuteRemoveIngredientCommand);
            FilterByIngredientsCommand = new Command(async () => await ExecuteFilterByIngredientsCommand());
            LoadRecipesCommand = new Command(async () => await LoadRecipesAsync());
        }

        // Loads all recipes and updates the search results collection.
        public async Task LoadRecipesAsync()
        {
            await ExecuteWithBusyIndicator(async () =>
            {
                var recipes = await _recipeService.GetAllRecipesAsync();
                SearchResults.Clear();
                foreach (var recipe in recipes)
                {
                    SearchResults.Add(recipe);
                }
            });
        }

        // Executes a search using the search query; if empty, loads all recipes.
        private async Task ExecuteSearchCommand()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                await LoadRecipesAsync();
                return;
            }

            await ExecuteWithBusyIndicator(async () =>
            {
                var recipes = await _recipeService.SearchByTitleAsync(SearchQuery);
                SearchResults.Clear();
                foreach (var recipe in recipes)
                {
                    SearchResults.Add(recipe);
                }
            });
        }

        // Navigates to the recipe details page when a recipe is tapped.
        private async Task ExecuteRecipeTappedCommand(RecipeModel recipe)
        {
            if (recipe != null)
            {
                await Shell.Current.GoToAsync($"recipedetails?id={recipe.Id}");
            }
        }

        // Adds a new ingredient name to the ingredient filter list.
        private void ExecuteAddIngredientCommand()
        {
            if (string.IsNullOrWhiteSpace(NewIngredientName))
            {
                ErrorMessage = "Ingredient name is required";
                return;
            }

            Ingredients.Add(new Ingredient { Name = NewIngredientName });
            NewIngredientName = string.Empty;
        }

        // Removes the specified ingredient from the filter list.
        private void ExecuteRemoveIngredientCommand(Ingredient ingredient)
        {
            Ingredients.Remove(ingredient);
        }

        // Filters recipes based on the ingredients in the filter list.
        private async Task ExecuteFilterByIngredientsCommand()
        {
            if (Ingredients.Count == 0)
            {
                await LoadRecipesAsync();
                return;
            }

            await ExecuteWithBusyIndicator(async () =>
            {
                // Generate a distinct list of ingredient names (trimmed and in lower case).
                var ingredientNames = Ingredients
                    .Where(i => !string.IsNullOrWhiteSpace(i.Name))
                    .Select(i => i.Name.Trim().ToLower())
                    .Distinct()
                    .ToList();

                var recipes = await _recipeService.FilterByIngredientNamesAsync(ingredientNames);
                SearchResults.Clear();
                foreach (var recipe in recipes)
                {
                    SearchResults.Add(recipe);
                }
            });
        }
    }
}
