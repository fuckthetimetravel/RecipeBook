using System;
using Microsoft.Maui.Controls;
using RecipeBook.Extensions;
using RecipeBook.Services;
using RecipeBook.ViewModels;

namespace RecipeBook.Views
{
    public partial class RecipeDetailsPage : ContentPage
    {
        private RecipeDetailsViewModel _viewModel;

        public RecipeDetailsPage(RecipeService recipeService, AuthService authService)
        {
            InitializeComponent();
            _viewModel = new RecipeDetailsViewModel(recipeService, authService);
            BindingContext = _viewModel;
        }

        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            var recipeId = Shell.Current.CurrentState.GetQueryParameter("id");
            if (!string.IsNullOrEmpty(recipeId))
            {
                await _viewModel.LoadRecipeAsync(recipeId);
            }
        }
    }
}