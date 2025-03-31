using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using RecipeBook.Models;
using RecipeBook.Services;

namespace RecipeBook.ViewModels
{
    public class SearchRecipesViewModel : BaseViewModel
    {
        private readonly RecipeService _recipeService;
        private string _searchQuery;
        private ObservableCollection<RecipeModel> _searchResults;
        private ObservableCollection<Ingredient> _ingredients;
        private string _newIngredientName;

        public string SearchQuery
        {
            get => _searchQuery;
            set => SetProperty(ref _searchQuery, value);
        }

        public ObservableCollection<RecipeModel> SearchResults
        {
            get => _searchResults;
            set => SetProperty(ref _searchResults, value);
        }

        public ObservableCollection<Ingredient> Ingredients
        {
            get => _ingredients;
            set => SetProperty(ref _ingredients, value);
        }

        public string NewIngredientName
        {
            get => _newIngredientName;
            set => SetProperty(ref _newIngredientName, value);
        }

        public ICommand SearchCommand { get; }
        public ICommand RecipeTappedCommand { get; }
        public ICommand AddIngredientCommand { get; }
        public ICommand RemoveIngredientCommand { get; }
        public ICommand FilterByIngredientsCommand { get; }
        public ICommand LoadRecipesCommand { get; }

        public SearchRecipesViewModel(RecipeService recipeService)
        {
            _recipeService = recipeService;
            SearchResults = new ObservableCollection<RecipeModel>();
            Ingredients = new ObservableCollection<Ingredient>();

            SearchCommand = new Command(async () => await ExecuteSearchCommand());
            RecipeTappedCommand = new Command<RecipeModel>(async (recipe) => await ExecuteRecipeTappedCommand(recipe));
            AddIngredientCommand = new Command(ExecuteAddIngredientCommand);
            RemoveIngredientCommand = new Command<Ingredient>(ExecuteRemoveIngredientCommand);
            FilterByIngredientsCommand = new Command(async () => await ExecuteFilterByIngredientsCommand());
            LoadRecipesCommand = new Command(async () => await LoadRecipesAsync());
        }

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

        private async Task ExecuteRecipeTappedCommand(RecipeModel recipe)
        {
            if (recipe != null)
            {
                await Shell.Current.GoToAsync($"recipedetails?id={recipe.Id}");
            }
        }

        private void ExecuteAddIngredientCommand()
        {
            if (string.IsNullOrWhiteSpace(NewIngredientName))
            {
                ErrorMessage = "Ingredient name is required";
                return;
            }

            Ingredients.Add(new Ingredient
            {
                Name = NewIngredientName,
            });

            NewIngredientName = string.Empty;
        }

        private void ExecuteRemoveIngredientCommand(Ingredient ingredient)
        {
            Ingredients.Remove(ingredient);
        }

        private async Task ExecuteFilterByIngredientsCommand()
        {
            if (Ingredients.Count == 0)
            {
                await LoadRecipesAsync();
                return;
            }

            await ExecuteWithBusyIndicator(async () =>
            {
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