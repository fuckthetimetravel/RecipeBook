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
    }
}