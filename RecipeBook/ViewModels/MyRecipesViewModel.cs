using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using RecipeBook.Models;
using RecipeBook.Services;

namespace RecipeBook.ViewModels
{
    public class MyRecipesViewModel : BaseViewModel
    {
        private readonly RecipeService _recipeService;
        private readonly AuthService _authService;
        private ObservableCollection<RecipeModel> _recipes;

        public ObservableCollection<RecipeModel> Recipes
        {
            get => _recipes;
            set => SetProperty(ref _recipes, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand AddRecipeCommand { get; }
        public ICommand RecipeTappedCommand { get; }
        public ICommand LoadRecipesCommand { get; }

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

        public async Task LoadRecipesAsync()
        {
            await ExecuteWithBusyIndicator(async () =>
            {
                if (!_authService.IsAuthenticated)
                {
                    ErrorMessage = "Please sign in to view your recipes";
                    return;
                }

                var recipes = await _recipeService.GetUserRecipesAsync(_authService.CurrentUser.Id);

                Recipes.Clear();
                foreach (var recipe in recipes)
                {
                    Recipes.Add(recipe);
                }
            });
        }

        private async Task ExecuteAddRecipeCommand()
        {
            await Shell.Current.GoToAsync("/addrecipe");
        }

        private async Task ExecuteRecipeTappedCommand(RecipeModel recipe)
        {
            if (recipe != null)
            {
                await Shell.Current.GoToAsync($"recipedetails?id={recipe.Id}");
            }
        }
    }
}