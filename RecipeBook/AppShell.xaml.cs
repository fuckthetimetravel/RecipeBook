using Microsoft.Maui.Controls;
using RecipeBook.Views;
using System;

namespace RecipeBook
{
    // AppShell is the main navigation container for the RecipeBook app.
    // It registers the routes for navigating between different pages.
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for navigation so that pages can be navigated by route name.
            Routing.RegisterRoute("login", typeof(LoginPage));
            Routing.RegisterRoute("register", typeof(RegistrPage));
            Routing.RegisterRoute("profile", typeof(ProfilePage));
            Routing.RegisterRoute("addrecipe", typeof(AddRecipePage));
            Routing.RegisterRoute("myrecipes", typeof(MyRecipesPage));
            Routing.RegisterRoute("searchrecipes", typeof(SearchRecipesPage));
            Routing.RegisterRoute("favoriterecipes", typeof(FavoriteRecipesPage));
            Routing.RegisterRoute("recipedetails", typeof(RecipeDetailsPage));
            Routing.RegisterRoute("editrecipe", typeof(EditRecipePage));
        }
    }
}
