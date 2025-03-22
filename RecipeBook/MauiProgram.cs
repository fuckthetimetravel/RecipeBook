using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using RecipeBook.Services;
using RecipeBook.ViewModels;
using RecipeBook.Views;

namespace RecipeBook
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register services
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<RecipeService>();

            // Register ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegistrationViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<AddRecipeViewModel>();
            builder.Services.AddTransient<MyRecipesViewModel>();
            builder.Services.AddTransient<SearchRecipesViewModel>();
            builder.Services.AddTransient<FavoriteRecipesViewModel>();
            builder.Services.AddTransient<RecipeDetailsViewModel>();

            // Register Views
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegistrPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<AddRecipePage>();
            builder.Services.AddTransient<MyRecipesPage>();
            builder.Services.AddTransient<SearchRecipesPage>();
            builder.Services.AddTransient<FavoriteRecipesPage>();
            builder.Services.AddTransient<RecipeDetailsPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}