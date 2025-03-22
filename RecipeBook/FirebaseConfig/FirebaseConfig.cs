using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBook.FirebaseConfig
{
    public static class FirebaseConfig
    {
        // Замените на ваши данные Firebase
        public const string ApiKey = "AIzaSyB341R8lYwH-HLeks2T-8lCnqBMaP64q4k";
        public const string AuthDomain = "recipebookproj-dc40a.firebaseapp.com";
        public const string ProjectId = "recipebookproj-dc40a";
        public const string DatabaseUrl = "https://recipebookproj-dc40a-default-rtdb.firebaseio.com/";

        // Базовые URL для REST API
        public const string AuthBaseUrl = "https://identitytoolkit.googleapis.com/v1/";

        // Методы для формирования URL запросов
        public static string GetSignUpUrl() => $"{AuthBaseUrl}accounts:signUp?key={ApiKey}";
        public static string GetSignInUrl() => $"{AuthBaseUrl}accounts:signInWithPassword?key={ApiKey}";


        // URL для Realtime 

        public static string GetUserUrl(string userId) => $"{DatabaseUrl}users/{userId}.json";
        public static string GetRecipesUrl() => $"{DatabaseUrl}recipes.json";
        public static string GetRecipeUrl(string recipeId) => $"{DatabaseUrl}recipes/{recipeId}.json";
        public static string GetUserRecipesUrl(string userId) => $"{DatabaseUrl}recipes.json?orderBy=\"authorId\"&equalTo=\"{userId}\"";
        public static string GetUserFavoritesUrl(string userId) => $"{DatabaseUrl}users/{userId}/favoriteRecipes.json";
    }
}