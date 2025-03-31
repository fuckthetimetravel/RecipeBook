using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using RecipeBook.Models;
using RecipeBook.Services;

namespace RecipeBook.ViewModels
{
    [QueryProperty(nameof(RecipeId), "id")]
    public class EditRecipeViewModel : BaseViewModel
    {
        private readonly RecipeService _recipeService;
        private readonly AuthService _authService;

        private string _recipeId;
        private string _title;
        private string _description;
        private string _selectedImageBase64;
        private FileResult _selectedImage;
        private string _newIngredientName;
        private string _newIngredientQuantity;
        private string _newStepText;
        private string _errorMessage;
        private RecipeModel _originalRecipe;

        public string RecipeId
        {
            get => _recipeId;
            set
            {
                if (SetProperty(ref _recipeId, value) && !string.IsNullOrEmpty(value))
                {
                    // Load the recipe when the ID is set
                    LoadRecipeAsync(value).ConfigureAwait(false);
                }
            }
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string SelectedImageBase64
        {
            get => _selectedImageBase64;
            set => SetProperty(ref _selectedImageBase64, value);
        }

        public FileResult SelectedImage
        {
            get => _selectedImage;
            set => SetProperty(ref _selectedImage, value);
        }

        public string NewIngredientName
        {
            get => _newIngredientName;
            set => SetProperty(ref _newIngredientName, value);
        }

        public string NewIngredientQuantity
        {
            get => _newIngredientQuantity;
            set => SetProperty(ref _newIngredientQuantity, value);
        }

        public string NewStepText
        {
            get => _newStepText;
            set => SetProperty(ref _newStepText, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ObservableCollection<Ingredient> Ingredients { get; } = new ObservableCollection<Ingredient>();
        public ObservableCollection<RecipeStep> Steps { get; } = new ObservableCollection<RecipeStep>();

        public ICommand PickImageCommand { get; }
        public ICommand AddIngredientCommand { get; }
        public ICommand RemoveIngredientCommand { get; }
        public ICommand AddStepCommand { get; }
        public ICommand RemoveStepCommand { get; }
        public ICommand SaveRecipeCommand { get; }

        public EditRecipeViewModel(RecipeService recipeService, AuthService authService)
        {
            _recipeService = recipeService;
            _authService = authService;

            PickImageCommand = new Command(async () => await ExecutePickImageCommand());
            AddIngredientCommand = new Command(ExecuteAddIngredientCommand);
            RemoveIngredientCommand = new Command<Ingredient>(ExecuteRemoveIngredientCommand);
            AddStepCommand = new Command(ExecuteAddStepCommand);
            RemoveStepCommand = new Command<RecipeStep>(ExecuteRemoveStepCommand);
            SaveRecipeCommand = new Command(async () => await ExecuteSaveRecipeCommand());
        }

        private async Task LoadRecipeAsync(string recipeId)
        {
            if (string.IsNullOrEmpty(recipeId))
            {
                ErrorMessage = "Recipe ID not provided";
                await Shell.Current.GoToAsync("..");
                return;
            }

            await ExecuteWithBusyIndicator(async () =>
            {
                try
                {
                    _originalRecipe = await _recipeService.GetRecipeAsync(recipeId);

                    if (_originalRecipe == null)
                    {
                        ErrorMessage = "Recipe not found";
                        await Shell.Current.GoToAsync("..");
                        return;
                    }

                    // Check if the current user is the author
                    if (!_authService.IsAuthenticated ||
                        _authService.CurrentUser == null ||
                        _originalRecipe.AuthorId != _authService.CurrentUser.Id)
                    {
                        await Shell.Current.DisplayAlert("Permission Denied",
                            "You can only edit recipes that you created.", "OK");
                        await Shell.Current.GoToAsync("..");
                        return;
                    }

                    // Populate the form with recipe data
                    Title = _originalRecipe.Title;
                    Description = _originalRecipe.Description;
                    SelectedImageBase64 = _originalRecipe.ImageBase64;

                    // Clear and populate ingredients
                    Ingredients.Clear();
                    foreach (var ingredient in _originalRecipe.Ingredients)
                    {
                        Ingredients.Add(ingredient);
                    }

                    // Clear and populate steps
                    Steps.Clear();
                    foreach (var step in _originalRecipe.Steps)
                    {
                        Steps.Add(step);
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Failed to load recipe: {ex.Message}";
                    await Shell.Current.GoToAsync("..");
                }
            });
        }

        private async Task ExecutePickImageCommand()
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select a photo"
                });

                if (result != null)
                {
                    SelectedImage = result;
                    SelectedImageBase64 = await _recipeService.ConvertImageToBase64Async(result);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error selecting photo: {ex.Message}";
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
                Quantity = NewIngredientQuantity ?? string.Empty
            });

            NewIngredientName = string.Empty;
            NewIngredientQuantity = string.Empty;
            ErrorMessage = string.Empty;
        }

        private void ExecuteRemoveIngredientCommand(Ingredient ingredient)
        {
            if (ingredient != null)
            {
                Ingredients.Remove(ingredient);
            }
        }

        private void ExecuteAddStepCommand()
        {
            if (string.IsNullOrWhiteSpace(NewStepText))
            {
                ErrorMessage = "Step description is required";
                return;
            }

            Steps.Add(new RecipeStep
            {
                Text = NewStepText
            });

            NewStepText = string.Empty;
            ErrorMessage = string.Empty;
        }

        private void ExecuteRemoveStepCommand(RecipeStep step)
        {
            if (step != null)
            {
                Steps.Remove(step);
            }
        }

        private async Task ExecuteSaveRecipeCommand()
        {
            await ExecuteWithBusyIndicator(async () =>
            {
                if (string.IsNullOrWhiteSpace(Title))
                {
                    ErrorMessage = "Recipe title is required";
                    return;
                }

                try
                {
                    // Update the recipe with new values
                    var updatedRecipe = new RecipeModel
                    {
                        Id = RecipeId,
                        Title = Title,
                        Description = Description ?? string.Empty,
                        Ingredients = new List<Ingredient>(Ingredients),
                        Steps = new List<RecipeStep>(Steps),
                        ImageBase64 = SelectedImageBase64,
                        AuthorId = _originalRecipe.AuthorId // Preserve the original author
                    };

                    await _recipeService.UpdateRecipeAsync(updatedRecipe);

                    await Shell.Current.DisplayAlert("Success", "Recipe updated successfully", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Failed to update recipe: {ex.Message}";
                }
            });
        }
    }
}