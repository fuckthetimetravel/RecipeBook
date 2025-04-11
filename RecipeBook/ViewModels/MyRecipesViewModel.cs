using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using RecipeBook.Models;
using RecipeBook.Services;

namespace RecipeBook.ViewModels
{
    // ViewModel for displaying the current user's recipes.
    public class MyRecipesViewModel : BaseViewModel
    {
        // Services for recipe operations and authentication.
        private readonly RecipeService _recipeService;
        private readonly AuthService _authService;

        // Backing field for the collection of recipes.
        private ObservableCollection<RecipeModel> _recipes;

        // Collection of the user's recipes for binding to the UI.
        public ObservableCollection<RecipeModel> Recipes
        {
            get => _recipes;
            set => SetProperty(ref _recipes, value);
        }

        // Command to refresh the recipes list.
        public ICommand RefreshCommand { get; }

        // Command to navigate to the add recipe page.
        public ICommand AddRecipeCommand { get; }

        // Command triggered when a recipe is tapped.
        public ICommand RecipeTappedCommand { get; }

        // Command to load the recipes list.
        public ICommand LoadRecipesCommand { get; }

        // Constructor that initializes services and commands.
        public MyRecipesViewModel(RecipeService recipeService, AuthService authService)
        {
            _recipeService = recipeService;
            _authService = authService;
            Recipes = new ObservableCollection<RecipeModel>();

            RefreshCommand = new Command(async () => await LoadRecipesAsync());
            AddRecipeCommand = new Command(async () => await ExecuteAddRecipeCommand());
            RecipeTappedCommand = new Command<RecipeModel>(async (recipe) => await ExecuteRecipeTappedCommand(recipe));
            LoadRecipesCommand = new Command(async () => await LoadRecipesAsync());
        }

        // Loads the current user's recipes.
        public async Task LoadRecipesAsync()
        {
            await ExecuteWithBusyIndicator(async () =>
            {
                // Verify that a user is signed in.
                if (!_authService.IsAuthenticated)
                {
                    ErrorMessage = "Please sign in to view your recipes";
                    return;
                }

                // Retrieve the user's recipes.
                var recipes = await _recipeService.GetUserRecipesAsync(_authService.CurrentUser.Id);

                // Update the observable collection.
                Recipes.Clear();
                foreach (var recipe in recipes)
                {
                    Recipes.Add(recipe);
                }
            });
        }

        // Navigates to the add recipe page.
        private async Task ExecuteAddRecipeCommand()
        {
            await Shell.Current.GoToAsync("/addrecipe");
        }

        // Navigates to the recipe details page when a recipe is tapped.
        private async Task ExecuteRecipeTappedCommand(RecipeModel recipe)
        {
            if (recipe != null)
            {
                await Shell.Current.GoToAsync($"recipedetails?id={recipe.Id}");
            }
        }
    }
}
