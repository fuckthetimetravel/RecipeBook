using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using RecipeBook.Services;

namespace RecipeBook.ViewModels
{
    public class AddRecipeViewModel : BaseViewModel
    {
        public string RecipeTitle { get; set; }
        public string RecipeDescription { get; set; }
        public ObservableCollection<string> Ingredients { get; set; } = new();
        public ObservableCollection<string> Steps { get; set; } = new();

        public ICommand AddIngredientCommand => new Command(async () => await AddIngredientAsync());
        public ICommand AddStepCommand => new Command(async () => await AddStepAsync());
        public ICommand SaveRecipeCommand => new Command(async () => await SaveRecipeAsync());

        private async Task AddIngredientAsync()
        {
            string ingredient = await Application.Current.MainPage.DisplayPromptAsync("New Ingredient", "Enter ingredient:");
            if (!string.IsNullOrWhiteSpace(ingredient))
            {
                Ingredients.Add(ingredient);
            }
        }

        private async Task AddStepAsync()
        {
            string step = await Application.Current.MainPage.DisplayPromptAsync("New Step", "Enter step:");
            if (!string.IsNullOrWhiteSpace(step))
            {
                Steps.Add(step);
            }
        }

        private async Task SaveRecipeAsync()
        {
            if (string.IsNullOrWhiteSpace(RecipeTitle) || string.IsNullOrWhiteSpace(RecipeDescription) ||
                Ingredients.Count == 0 || Steps.Count == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "All fields are required.", "OK");
                return;
            }

            var success = await AuthService.PostRecipeAsync(RecipeTitle, RecipeDescription, string.Join(", ", Ingredients), string.Join(". ", Steps));

            if (success)
            {
                await Application.Current.MainPage.DisplayAlert("Success", "Recipe added!", "OK");
                await Shell.Current.GoToAsync("//profile");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to add recipe.", "OK");
            }
        }
    }
}
