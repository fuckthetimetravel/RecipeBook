using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using RecipeBook.Models;
using RecipeBook.Services;

namespace RecipeBook.ViewModels
{
    public class FavoriteRecipesViewModel : BaseViewModel
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
        public ICommand RecipeTappedCommand { get; }
        public ICommand RemoveFromFavoritesCommand { get; }
        public ICommand LoadFavoriteRecipesCommand { get; }

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

        public async Task LoadFavoriteRecipesAsync()
        {
            await ExecuteWithBusyIndicator(async () =>
            {
                if (!_authService.IsAuthenticated)
                {
                    ErrorMessage = "Please sign in to view your favorite recipes";
                    return;
                }

                var recipes = await _recipeService.GetFavoriteRecipesAsync();

                Recipes.Clear();
                foreach (var recipe in recipes)
                {
                    Recipes.Add(recipe);
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

        private async Task ExecuteRemoveFromFavoritesCommand(RecipeModel recipe)
        {
            if (recipe != null)
            {
                await ExecuteWithBusyIndicator(async () =>
                {
                    await _recipeService.RemoveFromFavoritesAsync(recipe.Id);
                    Recipes.Remove(recipe);
                });
            }
        }
    }
}