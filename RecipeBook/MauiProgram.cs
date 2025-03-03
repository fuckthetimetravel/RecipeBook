using Microsoft.Extensions.Logging;
using RecipeBook.FirebaseConfig;
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

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<FirestoreAuthService>();
            builder.Services.AddSingleton<FirestoreService>();

            builder.Services.AddTransient<RegistrationViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<SearchRecipesViewModel>();
            builder.Services.AddTransient<AddRecipeViewModel>();

            builder.Services.AddTransient<RegistrPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<SearchRecipesPage>();
            builder.Services.AddTransient<AddRecipePage>();


            return builder.Build();
        }
    }
}
