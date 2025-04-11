using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using RecipeBook.Models;
using RecipeBook.Services;

namespace RecipeBook.ViewModels
{
    // ViewModel for adding a new recipe
    public class AddRecipeViewModel : BaseViewModel
    {
        // Service used to perform recipe-related operations
        private readonly RecipeService _recipeService;
        // Service used to retrieve the current location
        private readonly LocationService _locationService;

        // Backing fields for properties
        private string _title;
        private string _description;
        private ObservableCollection<Ingredient> _ingredients;
        private ObservableCollection<RecipeStep> _steps;
        private string _newIngredientName;
        private string _newIngredientQuantity;
        private string _newStepText;
        private FileResult _selectedImage;
        private string _selectedImageBase64;
        private string _location;

        // Public properties for data binding
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

        public ObservableCollection<Ingredient> Ingredients
        {
            get => _ingredients;
            set => SetProperty(ref _ingredients, value);
        }

        public ObservableCollection<RecipeStep> Steps
        {
            get => _steps;
            set => SetProperty(ref _steps, value);
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

        public FileResult SelectedImage
        {
            get => _selectedImage;
            set => SetProperty(ref _selectedImage, value);
        }

        public string SelectedImageBase64
        {
            get => _selectedImageBase64;
            set => SetProperty(ref _selectedImageBase64, value);
        }

        public string Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        // Commands for user interactions in the view
        public ICommand GetCurrentLocationCommand { get; }
        public ICommand AddIngredientCommand { get; }
        public ICommand RemoveIngredientCommand { get; }
        public ICommand AddStepCommand { get; }
        public ICommand RemoveStepCommand { get; }
        public ICommand PickImageCommand { get; }
        public ICommand TakePhotoCommand { get; }
        public ICommand SaveRecipeCommand { get; }

        // Constructor initializing services, collections, and commands
        public AddRecipeViewModel(RecipeService recipeService, LocationService locationService)
        {
            _recipeService = recipeService;
            _locationService = locationService;
            Title = "Add Recipe";
            Ingredients = new ObservableCollection<Ingredient>();
            Steps = new ObservableCollection<RecipeStep>();

            // Initialize command implementations
            AddIngredientCommand = new Command(ExecuteAddIngredientCommand);
            RemoveIngredientCommand = new Command<Ingredient>(ExecuteRemoveIngredientCommand);
            AddStepCommand = new Command(ExecuteAddStepCommand);
            RemoveStepCommand = new Command<RecipeStep>(ExecuteRemoveStepCommand);
            PickImageCommand = new Command(async () => await ExecutePickImageCommand());
            TakePhotoCommand = new Command(async () => await ExecuteTakePhotoCommand());
            SaveRecipeCommand = new Command(async () => await ExecuteSaveRecipeCommand());
            GetCurrentLocationCommand = new Command(async () => await ExecuteGetCurrentLocationCommand());
        }

        // Adds a new ingredient to the recipe
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
                Quantity = NewIngredientQuantity
            });

            // Clear input fields after adding
            NewIngredientName = string.Empty;
            NewIngredientQuantity = string.Empty;
        }

        // Removes a specified ingredient from the recipe
        private void ExecuteRemoveIngredientCommand(Ingredient ingredient)
        {
            Ingredients.Remove(ingredient);
        }

        // Adds a new step to the recipe process
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

            // Clear the step input field after adding
            NewStepText = string.Empty;
        }

        // Removes a specified step from the recipe process
        private void ExecuteRemoveStepCommand(RecipeStep step)
        {
            Steps.Remove(step);
        }

        // Command to pick an image from the device's photo library
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
                ErrorMessage = $"Error picking image: {ex.Message}";
            }
        }

        // Command to take a photo using the device's camera
        private async Task ExecuteTakePhotoCommand()
        {
            try
            {
                var result = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Take a photo"
                });

                if (result != null)
                {
                    SelectedImage = result;
                    SelectedImageBase64 = await _recipeService.ConvertImageToBase64Async(result);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error taking photo: {ex.Message}";
            }
        }

        // Command to save the new recipe by sending it to the RecipeService
        private async Task ExecuteSaveRecipeCommand()
        {
            await ExecuteWithBusyIndicator(async () =>
            {
                if (string.IsNullOrWhiteSpace(_title))
                {
                    ErrorMessage = "Recipe title is required";
                    return;
                }

                // Create a new RecipeModel instance from the entered data
                var recipe = new RecipeModel
                {
                    Title = _title,
                    Description = _description ?? string.Empty,
                    Ingredients = new List<Ingredient>(Ingredients),
                    Steps = new List<RecipeStep>(Steps),
                    ImageBase64 = _selectedImageBase64,
                    Location = _location
                };

                // Call the service to add the recipe to the database
                await _recipeService.AddRecipeAsync(recipe);

                // Reset form fields after saving
                _title = string.Empty;
                _description = string.Empty;
                _location = string.Empty;
                Ingredients.Clear();
                Steps.Clear();
                SelectedImage = null;
                SelectedImageBase64 = null;

                // Notify property changes for bound fields
                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(Description));
                OnPropertyChanged(nameof(Location));
                OnPropertyChanged(nameof(SelectedImage));
                OnPropertyChanged(nameof(SelectedImageBase64));

                await Shell.Current.DisplayAlert("Success", "Recipe added successfully", "OK");
            });
        }

        // Command to retrieve the current location and update the Location property
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
