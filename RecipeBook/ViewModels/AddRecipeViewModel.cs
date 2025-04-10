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
    public class AddRecipeViewModel : BaseViewModel
    {
        private readonly RecipeService _recipeService;
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
        private readonly LocationService _locationService;

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


        public ICommand GetCurrentLocationCommand { get; }
        public ICommand AddIngredientCommand { get; }
        public ICommand RemoveIngredientCommand { get; }
        public ICommand AddStepCommand { get; }
        public ICommand RemoveStepCommand { get; }
        public ICommand PickImageCommand { get; }
        public ICommand TakePhotoCommand { get; }
        public ICommand SaveRecipeCommand { get; }

        public AddRecipeViewModel(RecipeService recipeService, LocationService locationService)
        {
            _recipeService = recipeService;
            _locationService = locationService;
            Title = "Add Recipe";
            Ingredients = new ObservableCollection<Ingredient>();
            Steps = new ObservableCollection<RecipeStep>();

            AddIngredientCommand = new Command(ExecuteAddIngredientCommand);
            RemoveIngredientCommand = new Command<Ingredient>(ExecuteRemoveIngredientCommand);
            AddStepCommand = new Command(ExecuteAddStepCommand);
            RemoveStepCommand = new Command<RecipeStep>(ExecuteRemoveStepCommand);
            PickImageCommand = new Command(async () => await ExecutePickImageCommand());
            TakePhotoCommand = new Command(async () => await ExecuteTakePhotoCommand());
            SaveRecipeCommand = new Command(async () => await ExecuteSaveRecipeCommand());
            GetCurrentLocationCommand = new Command(async () => await ExecuteGetCurrentLocationCommand());
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
                Quantity = NewIngredientQuantity
            });

            NewIngredientName = string.Empty;
            NewIngredientQuantity = string.Empty;
        }

        private void ExecuteRemoveIngredientCommand(Ingredient ingredient)
        {
            Ingredients.Remove(ingredient);
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
        }

        private void ExecuteRemoveStepCommand(RecipeStep step)
        {
            Steps.Remove(step);
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
                ErrorMessage = $"Error picking image: {ex.Message}";
            }
        }

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

        private async Task ExecuteSaveRecipeCommand()
        {
            await ExecuteWithBusyIndicator(async () =>
            {
                if (string.IsNullOrWhiteSpace(_title))
                {
                    ErrorMessage = "Recipe title is required";
                    return;
                }

                var recipe = new RecipeModel
                {
                    Title = _title,
                    Description = _description ?? string.Empty,
                    Ingredients = new List<Ingredient>(Ingredients),
                    Steps = new List<RecipeStep>(Steps),
                    ImageBase64 = _selectedImageBase64, // Устанавливаем изображение для всего рецепта
                    Location = _location // Добавляем местоположение
                };

                await _recipeService.AddRecipeAsync(recipe);

                // Reset form
                _title = string.Empty;
                _description = string.Empty;
                _location = string.Empty; // Reset location
                Ingredients.Clear();
                Steps.Clear();
                SelectedImage = null;
                SelectedImageBase64 = null;
                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(Description));
                OnPropertyChanged(nameof(Location)); // Notify location change
                OnPropertyChanged(nameof(SelectedImage));
                OnPropertyChanged(nameof(SelectedImageBase64));

                await Shell.Current.DisplayAlert("Success", "Recipe added successfully", "OK");
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