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
        public const string ApiKey = "AIzaSyALxkiSSehbYeO8YmL621_e0xpaTsqFybg";
        public const string AuthDomain = "recipebook-15d21.firebaseapp.com";
        public const string ProjectId = "recipebook-15d21";
        public const string DatabaseUrl = "https://recipebook-15d21-default-rtdb.europe-west1.firebasedatabase.app/";

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