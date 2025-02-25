using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RecipeBook.Models;

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
        /// Универсальный метод для отправки данных в Firebase Realtime Database
        /// </summary>
        public async Task<bool> PostDataAsync<T>(string url, T data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{FirebaseConfig.RealtimeDatabaseUrl}/{url}.json", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to POST data: {error}");
            }

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Добавить рецепт в Realtime Database
        /// </summary>
        public async Task AddRecipeAsync(string userId, Recipe recipe)
        {
            string url = $"{FirebaseConfig.RealtimeDatabaseUrl}/users/{userId}/recipes.json";

            var json = JsonSerializer.Serialize(recipe);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error adding recipe: {error}");
            }
        }
    }
}
