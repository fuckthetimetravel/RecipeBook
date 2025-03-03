using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using RecipeBook.Models;
using RecipeBook.Services;

namespace RecipeBook.ViewModels
{
    public class SearchRecipesViewModel : BaseViewModel
    {
        public ObservableCollection<RecipeModel> Recipes { get; set; } = new();
        public string SearchQuery { get; set; }
        public ICommand SearchCommand => new Command(FilterRecipes);

        public SearchRecipesViewModel()
        {
            LoadRecipesAsync();
        }

        private async Task LoadRecipesAsync()
        {
            var recipes = await AuthService.GetRecipesAsync();
            if (recipes != null)
            {
                Recipes.Clear();
                foreach (var recipe in recipes)
                {
                    Recipes.Add(recipe);
                }
            }
        }

        private void FilterRecipes()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                LoadRecipesAsync();
            }
            else
            {
                var filtered = Recipes.Where(r =>
                    r.Title.ToLower().Contains(SearchQuery.ToLower()) ||
                    r.Description.ToLower().Contains(SearchQuery.ToLower())).ToList();

                Recipes.Clear();
                foreach (var recipe in filtered)
                {
                    Recipes.Add(recipe);
                }
            }
        }
    }
}
