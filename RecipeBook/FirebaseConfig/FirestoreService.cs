using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json;
using System.Text;
using RecipeBook.Models;
using System.Threading.Tasks;

namespace RecipeBook.FirebaseConfig
{
    public class FirestoreService
    {
        private readonly HttpClient _httpClient;

        public FirestoreService()
        {
            _httpClient = FirebaseConfig.GetHttpClient();
        }

        /// <summary>
        /// Добавить рецепт в Firestore
        /// </summary>
        public async Task AddRecipeAsync(string userId, Recipe recipe)
        {
            string url = $"{FirebaseConfig.FirestoreBaseUrl}/users/{userId}/recipes";

            var json = JsonSerializer.Serialize(new { fields = recipe.ToFirestoreFormat() });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error adding recipe");
            }
        }

        /// <summary>
        /// Получить список рецептов пользователя
        /// </summary>
        public async Task<List<Recipe>> GetUserRecipesAsync(string userId)
        {
            string url = $"{FirebaseConfig.FirestoreBaseUrl}/users/{userId}/recipes";

            var response = await _httpClient.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            return FirestoreParser.ParseRecipes(json);
        }

        /// <summary>
        /// Удалить рецепт
        /// </summary>
        public async Task DeleteRecipeAsync(string userId, string recipeId)
        {
            string url = $"{FirebaseConfig.FirestoreBaseUrl}/users/{userId}/recipes/{recipeId}";

            var response = await _httpClient.DeleteAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error deleting recipe");
            }
        }
    }
}
