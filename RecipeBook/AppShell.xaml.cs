using Microsoft.Maui.Controls;
using RecipeBook.Views;
using System;

namespace RecipeBook
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for navigation
            Routing.RegisterRoute("login", typeof(LoginPage));
            Routing.RegisterRoute("register", typeof(RegistrPage));
            Routing.RegisterRoute("profile", typeof(ProfilePage));
            Routing.RegisterRoute("addrecipe", typeof(AddRecipePage));
            Routing.RegisterRoute("myrecipes", typeof(MyRecipesPage));
            Routing.RegisterRoute("searchrecipes", typeof(SearchRecipesPage));
            Routing.RegisterRoute("favoriterecipes", typeof(FavoriteRecipesPage));
            Routing.RegisterRoute("recipedetails", typeof(RecipeDetailsPage));
            Routing.RegisterRoute(nameof(RecipeDetailsPage), typeof(RecipeDetailsPage));
        }
    }
}