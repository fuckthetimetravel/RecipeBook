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
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<App>();


            // Register HttpClient as a singleton
            builder.Services.AddSingleton<HttpClient>();

            // Register services
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<RecipeService>();

            // Register ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegistrationViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<RecipeDetailsViewModel>();
            builder.Services.AddTransient<EditRecipeViewModel>();
            builder.Services.AddTransient<SearchRecipesViewModel>();
            builder.Services.AddTransient<MyRecipesViewModel>();
            builder.Services.AddTransient<AddRecipeViewModel>();

            // Register Views
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegistrPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<RecipeDetailsPage>();
            builder.Services.AddTransient<EditRecipePage>();
            builder.Services.AddTransient<SearchRecipesPage>();
            builder.Services.AddTransient<MyRecipesPage>();
            builder.Services.AddTransient<AddRecipePage>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}