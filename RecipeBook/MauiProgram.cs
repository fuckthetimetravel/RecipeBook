using Microsoft.Extensions.Logging;
using RecipeBook.Services;
using RecipeBook.ViewModels;
using RecipeBook.Views;

namespace RecipeBook
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            // Create the builder for the Maui application.
            var builder = MauiApp.CreateBuilder();

            // Configure the application with the main App class and fonts.
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    // Add custom fonts with their aliases.
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register the main App as a singleton so it persists for the lifetime of the application.
            builder.Services.AddSingleton<App>();

            // Register HttpClient as a singleton for making HTTP requests across the application.
            builder.Services.AddSingleton<HttpClient>();

            // Register application services as singletons.
            builder.Services.AddSingleton<AuthService>();      // Manages authentication and user data.
            builder.Services.AddSingleton<RecipeService>();    // Handles CRUD operations for recipes.
            builder.Services.AddSingleton<LocationService>();  // Provides access to location data.
            builder.Services.AddSingleton<SpeechService>();    // Speech to text

            // Register ViewModels with transient lifetime (a new instance is created every time).
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegistrationViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<RecipeDetailsViewModel>();
            builder.Services.AddTransient<EditRecipeViewModel>();
            builder.Services.AddTransient<SearchRecipesViewModel>();
            builder.Services.AddTransient<MyRecipesViewModel>();
            builder.Services.AddTransient<AddRecipeViewModel>();
            builder.Services.AddTransient<FavoriteRecipesViewModel>();

            // Register Views for navigation and UI rendering.
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegistrPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<RecipeDetailsPage>();
            builder.Services.AddTransient<EditRecipePage>();
            builder.Services.AddTransient<SearchRecipesPage>();
            builder.Services.AddTransient<MyRecipesPage>();
            builder.Services.AddTransient<AddRecipePage>();
            builder.Services.AddSingleton<FavoriteRecipesPage>();



#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}
