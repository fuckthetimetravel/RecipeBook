using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using RecipeBook.Models;
using RecipeBook.Services;

namespace RecipeBook.ViewModels
{
    // ViewModel for displaying and managing the details of a recipe.
    // The QueryProperty attribute maps the query parameter "id" to the RecipeId property.
    [QueryProperty(nameof(RecipeId), "id")]
    public class RecipeDetailsViewModel : BaseViewModel
    {
        // Services for handling recipe operations and user authentication.
        private readonly RecipeService _recipeService;
        private readonly AuthService _authService;
        private readonly SpeechService _speechService;

        // Backing fields.
        private RecipeModel _recipe;
        private bool _isOwner;
        private bool _isFavorite;
        private string _recipeId;
        private bool _isSpeaking;

        public string RecipeId
        {
            get => _recipeId;
            set
            {
                if (SetProperty(ref _recipeId, value) && !string.IsNullOrEmpty(value))
                {
                    // Load the recipe asynchronously when the ID is provided.
                    LoadRecipeAsync(value).ConfigureAwait(false);
                }
            }
        }

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

        public bool IsSpeaking
        {
            get => _isSpeaking;
            set => SetProperty(ref _isSpeaking, value);
        }

        // Commands for UI interactions.
        public ICommand EditRecipeCommand { get; }
        public ICommand DeleteRecipeCommand { get; }
        public ICommand ToggleFavoriteCommand { get; }
        public ICommand SpeakStepsCommand { get; }
        public ICommand StopSpeakingCommand { get; }
        public RecipeDetailsViewModel(RecipeService recipeService, AuthService authService, SpeechService speechService)
        {
            _recipeService = recipeService;
            _authService = authService;
            _speechService = speechService;

            // Initialize commands with asynchronous handlers.
            EditRecipeCommand = new Command(async () => await ExecuteEditRecipeCommand(), () => IsOwner);
            DeleteRecipeCommand = new Command(async () => await ExecuteDeleteRecipeCommand(), () => IsOwner);
            ToggleFavoriteCommand = new Command(async () => await ExecuteToggleFavoriteCommand());
            SpeakStepsCommand = new Command(async () => await ExecuteSpeakStepsCommand());
            StopSpeakingCommand = new Command(ExecuteStopSpeakingCommand);
        }

        // Loads the recipe details based on the provided recipe ID.
        public async Task LoadRecipeAsync(string recipeId)
        {
            if (string.IsNullOrEmpty(recipeId))
            {
                await Shell.Current.DisplayAlert("Error", "Recipe ID not provided", "OK");
                await Shell.Current.GoToAsync("..");
                return;
            }

            await ExecuteWithBusyIndicator(async () =>
            {
                try
                {
                    // Retrieve the recipe from the service.
                    Recipe = await _recipeService.GetRecipeAsync(recipeId);

                    if (Recipe == null)
                    {
                        await Shell.Current.DisplayAlert("Error", "Recipe not found", "OK");
                        await Shell.Current.GoToAsync("..");
                        return;
                    }

                    // Check if the current user is the owner of the recipe.
                    IsOwner = _authService.IsAuthenticated &&
                              _authService.CurrentUser != null &&
                              Recipe.AuthorId == _authService.CurrentUser.Id;

                    // Check if the recipe is in the user's favorites.
                    IsFavorite = _authService.IsAuthenticated &&
                                 _authService.CurrentUser?.FavoriteRecipes != null &&
                                 _authService.CurrentUser.FavoriteRecipes.Contains(recipeId);

                    // Update command execution status based on ownership.
                    ((Command)EditRecipeCommand).ChangeCanExecute();
                    ((Command)DeleteRecipeCommand).ChangeCanExecute();
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error", $"Failed to load recipe: {ex.Message}", "OK");
                    await Shell.Current.GoToAsync("..");
                }
            });
        }

        // Executes the edit recipe command by navigating to the edit page.
        private async Task ExecuteEditRecipeCommand()
        {
            if (!IsOwner)
            {
                await Shell.Current.DisplayAlert("Permission Denied",
                    "You can only edit recipes that you created.", "OK");
                return;
            }
            await Shell.Current.GoToAsync($"editrecipe?id={Recipe.Id}");
        }

        // Executes the delete recipe command after confirming with the user.
        private async Task ExecuteDeleteRecipeCommand()
        {
            if (!IsOwner)
            {
                await Shell.Current.DisplayAlert("Permission Denied",
                    "You can only delete recipes that you created.", "OK");
                return;
            }

            bool confirm = await Shell.Current.DisplayAlert("Confirm Delete",
                "Are you sure you want to delete this recipe?", "Yes", "No");

            if (confirm)
            {
                await ExecuteWithBusyIndicator(async () =>
                {
                    try
                    {
                        await _recipeService.DeleteRecipeAsync(Recipe.Id);
                        await Shell.Current.GoToAsync("..");
                    }
                    catch (Exception ex)
                    {
                        await Shell.Current.DisplayAlert("Error",
                            $"Failed to delete recipe: {ex.Message}", "OK");
                    }
                });
            }
        }

        // Toggles the favorite status of the recipe for the current user.
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
                try
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
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error",
                        $"Failed to update favorites: {ex.Message}", "OK");
                }
            });
        }

        private async Task ExecuteSpeakStepsCommand()
        {
            if (Recipe?.Steps == null || Recipe.Steps.Count == 0)
            {
                await Shell.Current.DisplayAlert("No Steps", "This recipe has no steps to read.", "OK");
                return;
            }

            IsSpeaking = true;
            await _speechService.SpeakRecipeStepsAsync(Recipe.Steps);
            IsSpeaking = _speechService.IsSpeaking;
        }

        private void ExecuteStopSpeakingCommand()
        {
            _speechService.StopSpeaking();
            IsSpeaking = false;
        }

        // Stops any ongoing speech and resets the speaking flag.
        public void OnDisappearing()
        {
            // Make sure to stop speaking when leaving the page
            _speechService.StopSpeaking();
            IsSpeaking = false;
        }
    }
}
