using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using RecipeBook.Models;
using RecipeBook.Services;

namespace RecipeBook.ViewModels
{
    public class RecipeDetailsViewModel : BaseViewModel
    {
        private readonly RecipeService _recipeService;
        private readonly AuthService _authService;
        private RecipeModel _recipe;
        private bool _isOwner;
        private bool _isFavorite;

        public RecipeModel Recipe
        {
            get => _recipe;
            set => SetProperty(ref _recipe, value);
        }

        public bool IsOwner
        {
            get => _isOwner;
            set => SetProperty(ref _isOwner, value);
        }

        public bool IsFavorite
        {
            get => _isFavorite;
            set => SetProperty(ref _isFavorite, value);
        }

        public ICommand EditRecipeCommand { get; }
        public ICommand DeleteRecipeCommand { get; }
        public ICommand ToggleFavoriteCommand { get; }

        public RecipeDetailsViewModel(RecipeService recipeService, AuthService authService)
        {
            _recipeService = recipeService;
            _authService = authService;

            EditRecipeCommand = new Command(async () => await ExecuteEditRecipeCommand());
            DeleteRecipeCommand = new Command(async () => await ExecuteDeleteRecipeCommand());
            ToggleFavoriteCommand = new Command(async () => await ExecuteToggleFavoriteCommand());
        }

        public async Task LoadRecipeAsync(string recipeId)
        {
            await ExecuteWithBusyIndicator(async () =>
            {
                Recipe = await _recipeService.GetRecipeAsync(recipeId);

                // Проверяем, является ли текущий пользователь автором рецепта
                IsOwner = _authService.IsAuthenticated && Recipe.AuthorId == _authService.CurrentUser.Id;

                // Проверяем, добавлен ли рецепт в избранное
                IsFavorite = _authService.IsAuthenticated &&
                             _authService.CurrentUser.FavoriteRecipes != null &&
                             _authService.CurrentUser.FavoriteRecipes.Contains(recipeId);
            });
        }

        private async Task ExecuteEditRecipeCommand()
        {
            // Переход на страницу редактирования рецепта
            await Shell.Current.GoToAsync($"editrecipe?id={Recipe.Id}");
        }

        private async Task ExecuteDeleteRecipeCommand()
        {
            bool confirm = await Shell.Current.DisplayAlert("Confirm Delete",
                "Are you sure you want to delete this recipe?", "Yes", "No");

            if (confirm)
            {
                await ExecuteWithBusyIndicator(async () =>
                {
                    await _recipeService.DeleteRecipeAsync(Recipe.Id);
                    await Shell.Current.GoToAsync("..");
                });
            }
        }

        private async Task ExecuteToggleFavoriteCommand()
        {
            if (!_authService.IsAuthenticated)
            {
                await Shell.Current.DisplayAlert("Authentication Required",
                    "Please sign in to add recipes to favorites", "OK");
                return;
            }

            await ExecuteWithBusyIndicator(async () =>
            {
                if (IsFavorite)
                {
                    await _recipeService.RemoveFromFavoritesAsync(Recipe.Id);
                }
                else
                {
                    await _recipeService.AddToFavoritesAsync(Recipe.Id);
                }

                IsFavorite = !IsFavorite;
            });
        }
    }
}