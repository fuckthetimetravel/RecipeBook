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
        private string _location;
        private readonly LocationService _locationService;

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

        public string Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        public ObservableCollection<Ingredient> Ingredients { get; } = new ObservableCollection<Ingredient>();
        public ObservableCollection<RecipeStep> Steps { get; } = new ObservableCollection<RecipeStep>();

        public ICommand GetCurrentLocationCommand { get; }
        public ICommand PickImageCommand { get; }
        public ICommand AddIngredientCommand { get; }
        public ICommand RemoveIngredientCommand { get; }
        public ICommand AddStepCommand { get; }
        public ICommand RemoveStepCommand { get; }
        public ICommand SaveRecipeCommand { get; }

        public EditRecipeViewModel(RecipeService recipeService, AuthService authService, LocationService locationService)
        {
            _recipeService = recipeService;
            _authService = authService;
            _locationService = locationService;

            Ingredients = new ObservableCollection<Ingredient>();
            Steps = new ObservableCollection<RecipeStep>();

            AddIngredientCommand = new Command(ExecuteAddIngredientCommand);
            RemoveIngredientCommand = new Command<Ingredient>(ExecuteRemoveIngredientCommand);
            AddStepCommand = new Command(ExecuteAddStepCommand);
            RemoveStepCommand = new Command<RecipeStep>(ExecuteRemoveStepCommand);
            PickImageCommand = new Command(async () => await ExecutePickImageCommand());
            SaveRecipeCommand = new Command(async () => await ExecuteSaveRecipeCommand());
            GetCurrentLocationCommand = new Command(async () => await ExecuteGetCurrentLocationCommand());
        }

        private async Task LoadRecipeAsync(string recipeId)
        {
            try
            {
                IsBusy = true;
                _originalRecipe = await _recipeService.GetRecipeAsync(recipeId);

                if (_originalRecipe != null)
                {
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

                    // Set properties
                    Title = _originalRecipe.Title;
                    Description = _originalRecipe.Description;
                    Location = _originalRecipe.Location; // Load location
                    SelectedImageBase64 = _originalRecipe.ImageBase64;

                    // Clear and add ingredients
                    Ingredients.Clear();
                    if (_originalRecipe.Ingredients != null)
                    {
                        foreach (var ingredient in _originalRecipe.Ingredients)
                        {
                            Ingredients.Add(ingredient);
                        }
                    }

                    // Clear and add steps
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
                try
                {
                    if (string.IsNullOrWhiteSpace(Title))
                    {
                        ErrorMessage = "Recipe title is required";
                        return;
                    }

                    var updatedRecipe = new RecipeModel
                    {
                        Id = RecipeId,
                        Title = Title,
                        Description = Description,
                        Ingredients = new List<Ingredient>(Ingredients),
                        Steps = new List<RecipeStep>(Steps),
                        ImageBase64 = SelectedImageBase64,
                        Location = Location, // Include location
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