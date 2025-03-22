using Microsoft.Maui.Controls;
using RecipeBook.ViewModels;
using System.Threading.Tasks;

namespace RecipeBook.Views
{
    public partial class SearchRecipesPage : ContentPage
    {
        private readonly SearchRecipesViewModel _viewModel;

        public SearchRecipesPage(SearchRecipesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Load recipes when page appears
            _viewModel.LoadRecipesCommand?.Execute(null);
        }

        private async void OnFilterButtonClicked(object sender, EventArgs e)
        {
            // Вместо Popup используем DisplayPromptAsync для ввода ингредиентов
            string ingredientName = await DisplayPromptAsync("Add Ingredient", "Enter ingredient name:");
            if (!string.IsNullOrWhiteSpace(ingredientName))
            {
                string quantity = await DisplayPromptAsync("Add Quantity", "Enter quantity (optional):", initialValue: "");

                // Добавляем ингредиент в ViewModel
                _viewModel.NewIngredientName = ingredientName;
                _viewModel.NewIngredientQuantity = quantity ?? "";
                _viewModel.AddIngredientCommand.Execute(null);

                // Спрашиваем, хочет ли пользователь добавить еще ингредиенты
                bool addMore = await DisplayAlert("Add More?", "Do you want to add more ingredients?", "Yes", "No");
                if (addMore)
                {
                    OnFilterButtonClicked(sender, e);
                }
                else
                {
                    // Применяем фильтр
                    _viewModel.FilterByIngredientsCommand.Execute(null);
                }
            }
        }
    }
}