using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using RecipeBook.Models;
using RecipeBook.Services;

namespace RecipeBook.ViewModels
{
    // ViewModel for editing an existing recipe.
    // The QueryProperty attribute maps the query parameter "id" to the RecipeId property.
    [QueryProperty(nameof(RecipeId), "id")]
    public class EditRecipeViewModel : BaseViewModel
    {
        // Services used by the ViewModel
        private readonly RecipeService _recipeService;
        private readonly AuthService _authService;
        private readonly LocationService _locationService;

        // Backing fields for properties
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
        private string _location;

        // RecipeId property. When set, triggers loading of the recipe data.
        public string RecipeId
        {
            get => _recipeId;
            set
            {
                if (SetProperty(ref _recipeId, value) && !string.IsNullOrEmpty(value))
                {
                    // Load the recipe data asynchronously.
                    LoadRecipeAsync(value).ConfigureAwait(false);
                }
            }
        }

        // Recipe title.
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        // Recipe description.
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        // Base64-encoded string for the selected image.
        public string SelectedImageBase64
        {
            get => _selectedImageBase64;
            set => SetProperty(ref _selectedImageBase64, value);
        }

        // File result representing the selected image.
        public FileResult SelectedImage
        {
            get => _selectedImage;
            set => SetProperty(ref _selectedImage, value);
        }

        // New ingredient name entered by the user.
        public string NewIngredientName
        {
            get => _newIngredientName;
            set => SetProperty(ref _newIngredientName, value);
        }

        // New ingredient quantity entered by the user.
        public string NewIngredientQuantity
        {
            get => _newIngredientQuantity;
            set => SetProperty(ref _newIngredientQuantity, value);
        }

        // New step text entered by the user.
        public string NewStepText
        {
            get => _newStepText;
            set => SetProperty(ref _newStepText, value);
        }

        // Error message for UI notifications.
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        // Location information for the recipe.
        public string Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        // Observable collections for ingredients and steps.
        public ObservableCollection<Ingredient> Ingredients { get; } = new ObservableCollection<Ingredient>();
        public ObservableCollection<RecipeStep> Steps { get; } = new ObservableCollection<RecipeStep>();

        // Commands bound to UI actions.
        public ICommand GetCurrentLocationCommand { get; }
        public ICommand PickImageCommand { get; }
        public ICommand AddIngredientCommand { get; }
        public ICommand RemoveIngredientCommand { get; }
        public ICommand AddStepCommand { get; }
        public ICommand RemoveStepCommand { get; }
        public ICommand SaveRecipeCommand { get; }

        // Constructor that initializes services and commands.
        public EditRecipeViewModel(RecipeService recipeService, AuthService authService, LocationService locationService)
        {
            _recipeService = recipeService;
            _authService = authService;
            _locationService = locationService;

            // Initialize commands with their corresponding action handlers.
            AddIngredientCommand = new Command(ExecuteAddIngredientCommand);
            RemoveIngredientCommand = new Command<Ingredient>(ExecuteRemoveIngredientCommand);
            AddStepCommand = new Command(ExecuteAddStepCommand);
            RemoveStepCommand = new Command<RecipeStep>(ExecuteRemoveStepCommand);
            PickImageCommand = new Command(async () => await ExecutePickImageCommand());
            SaveRecipeCommand = new Command(async () => await ExecuteSaveRecipeCommand());
            GetCurrentLocationCommand = new Command(async () => await ExecuteGetCurrentLocationCommand());
        }

        // Loads the recipe data for the specified recipe ID.
        private async Task LoadRecipeAsync(string recipeId)
        {
            try
            {
                IsBusy = true;
                _originalRecipe = await _recipeService.GetRecipeAsync(recipeId);

                if (_originalRecipe != null)
                {
                    // Verify that the current user is the author of the recipe.
                    if (!_authService.IsAuthenticated ||
                        _authService.CurrentUser == null ||
                        _originalRecipe.AuthorId != _authService.CurrentUser.Id)
                    {
                        await Shell.Current.DisplayAlert("Permission Denied",
                            "You can only edit recipes that you created.", "OK");
                        await Shell.Current.GoToAsync("..");
                        return;
                    }

                    // Populate ViewModel properties with the recipe data.
                    Title = _originalRecipe.Title;
                    Description = _originalRecipe.Description;
                    Location = _originalRecipe.Location;
                    SelectedImageBase64 = _originalRecipe.ImageBase64;

                    // Update ingredients collection.
                    Ingredients.Clear();
                    if (_originalRecipe.Ingredients != null)
                    {
                        foreach (var ingredient in _originalRecipe.Ingredients)
                        {
                            Ingredients.Add(ingredient);
                        }
                    }

                    // Update steps collection.
                    Steps.Clear();
                    if (_originalRecipe.Steps != null)
                    {
                        foreach (var step in _originalRecipe.Steps)
                        {
                            Steps.Add(step);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load recipe: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Allows the user to pick an image from the device.
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

        // Adds a new ingredient to the recipe.
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

            // Clear input fields.
            NewIngredientName = string.Empty;
            NewIngredientQuantity = string.Empty;
            ErrorMessage = string.Empty;
        }

        // Removes the specified ingredient from the collection.
        private void ExecuteRemoveIngredientCommand(Ingredient ingredient)
        {
            if (ingredient != null)
            {
                Ingredients.Remove(ingredient);
            }
        }

        // Adds a new step to the recipe.
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

            // Clear the step input field.
            NewStepText = string.Empty;
            ErrorMessage = string.Empty;
        }

        // Removes the specified step from the collection.
        private void ExecuteRemoveStepCommand(RecipeStep step)
        {
            if (step != null)
            {
                Steps.Remove(step);
            }
        }

        // Saves the updated recipe information.
        private async Task ExecuteSaveRecipeCommand()
        {
            await ExecuteWithBusyIndicator(async () =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(Title))
                    {
                        ErrorMessage = "Recipe title is required";
                        return;
                    }

                    // Build the updated recipe object.
                    var updatedRecipe = new RecipeModel
                    {
                        Id = RecipeId,
                        Title = Title,
                        Description = Description,
                        Ingredients = new List<Ingredient>(Ingredients),
                        Steps = new List<RecipeStep>(Steps),
                        ImageBase64 = SelectedImageBase64,
                        Location = Location,
                        AuthorId = _originalRecipe.AuthorId
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

        // Retrieves the current location and sets the Location property.
        private async Task ExecuteGetCurrentLocationCommand()
        {
            await ExecuteWithBusyIndicator(async () =>
            {
                try
                {
                    var location = await _locationService.GetCurrentLocationAsync();
                    if (!string.IsNullOrEmpty(location))
                    {
                        Location = location;
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Location Error", "Could not determine your current location.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Error getting location: {ex.Message}";
                }
            });
        }
    }
}
