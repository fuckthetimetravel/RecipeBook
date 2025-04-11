using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RecipeBook.Models;

namespace RecipeBook.Services
{
    public class RecipeService
    {
        // HttpClient instance for sending HTTP requests
        private HttpClient _httpClient;
        // Authentication service instance for managing user authentication
        private AuthService _authService;
        // Service for performing search operations on recipes
        private readonly RecipeSearchService _searchService = new RecipeSearchService();

        // Public property exposing the authentication service instance
        public AuthService AuthService { get; internal set; }

        // Constructor initializing HttpClient and setting the provided AuthService instance
        public RecipeService(AuthService authService)
        {
            _httpClient = new HttpClient();
            _authService = authService;
        }

        // Retrieves all recipes from the Firebase backend
        public async Task<List<RecipeModel>> GetAllRecipesAsync()
        {
            var url = $"{FirebaseConfig.FirebaseConfig.GetRecipesUrl()}?auth={_authService.AuthToken}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await _httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get recipes: {responseString}");
            }

            var recipes = new List<RecipeModel>();

            // Firebase Realtime Database returns an object with recipe IDs as keys
            if (responseString != "null")
            {
                var recipesDict = JsonSerializer.Deserialize<Dictionary<string, RecipeModel>>(responseString);
                if (recipesDict != null)
                {
                    foreach (var kvp in recipesDict)
                    {
                        var recipe = kvp.Value;
                        recipe.Id = kvp.Key; // Set recipe ID from the key
                        recipes.Add(recipe);
                    }
                }
            }

            return recipes;
        }

        // Retrieves recipes authored by a specific user based on user ID
        public async Task<List<RecipeModel>> GetUserRecipesAsync(string userId)
        {
            if (!_authService.IsAuthenticated)
            {
                throw new Exception("User is not authenticated");
            }

            var url = $"{FirebaseConfig.FirebaseConfig.DatabaseUrl}recipes.json?orderBy=\"authorId\"&equalTo=\"{userId}\"&auth={_authService.AuthToken}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await _httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get user recipes: {responseString}");
            }

            var recipes = new List<RecipeModel>();

            if (responseString != "null")
            {
                var recipesDict = JsonSerializer.Deserialize<Dictionary<string, RecipeModel>>(responseString);
                if (recipesDict != null)
                {
                    foreach (var kvp in recipesDict)
                    {
                        var recipe = kvp.Value;
                        recipe.Id = kvp.Key;
                        recipes.Add(recipe);
                    }
                }
            }

            return recipes;
        }

        // Retrieves the favorite recipes of the current user
        public async Task<List<RecipeModel>> GetFavoriteRecipesAsync()
        {
            if (!_authService.IsAuthenticated)
            {
                throw new Exception("User is not authenticated");
            }

            var favoriteIds = _authService.CurrentUser.FavoriteRecipes;
            if (favoriteIds == null || favoriteIds.Count == 0)
            {
                return new List<RecipeModel>();
            }

            var allRecipes = await GetAllRecipesAsync();
            return allRecipes.Where(r => favoriteIds.Contains(r.Id)).ToList();
        }

        // Retrieves a specific recipe by its ID
        public async Task<RecipeModel> GetRecipeAsync(string recipeId)
        {
            var url = $"{FirebaseConfig.FirebaseConfig.GetRecipeUrl(recipeId)}?auth={_authService.AuthToken}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await _httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get recipe: {responseString}");
            }

            if (responseString == "null")
            {
                throw new Exception($"Recipe not found: {recipeId}");
            }

            var recipe = JsonSerializer.Deserialize<RecipeModel>(responseString);
            recipe.Id = recipeId;

            return recipe;
        }

        // Adds a new recipe to the database
        public async Task<RecipeModel> AddRecipeAsync(RecipeModel recipe)
        {
            if (!_authService.IsAuthenticated)
            {
                throw new Exception("User is not authenticated");
            }

            // Set the current user as the author of the recipe
            recipe.AuthorId = _authService.CurrentUser.Id;

            var url = $"{FirebaseConfig.FirebaseConfig.GetRecipesUrl()}?auth={_authService.AuthToken}";
            var json = JsonSerializer.Serialize(recipe);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = content;

            var response = await _httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to add recipe: {responseString}");
            }

            // Firebase returns an object with a "name" property that represents the new recipe ID
            var result = JsonSerializer.Deserialize<Dictionary<string, string>>(responseString);
            recipe.Id = result["name"];

            return recipe;
        }

        // Updates an existing recipe in the database
        public async Task UpdateRecipeAsync(RecipeModel recipe)
        {
            if (!_authService.IsAuthenticated)
            {
                throw new Exception("User is not authenticated");
            }

            var url = $"{FirebaseConfig.FirebaseConfig.GetRecipeUrl(recipe.Id)}?auth={_authService.AuthToken}";
            var json = JsonSerializer.Serialize(recipe);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, url);
            request.Content = content;

            var response = await _httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to update recipe: {responseString}");
            }
        }

        // Deletes a recipe from the database
        public async Task DeleteRecipeAsync(string recipeId)
        {
            if (!_authService.IsAuthenticated)
            {
                throw new Exception("User is not authenticated");
            }

            var url = $"{FirebaseConfig.FirebaseConfig.GetRecipeUrl(recipeId)}?auth={_authService.AuthToken}";
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            var response = await _httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to delete recipe: {responseString}");
            }
        }

        // Converts an image file to a Base64 string
        public async Task<string> ConvertImageToBase64Async(FileResult file)
        {
            if (file == null)
                return null;

            using var stream = await file.OpenReadAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            byte[] bytes = memoryStream.ToArray();

            return Convert.ToBase64String(bytes);
        }

        // Adds a recipe to the current user's favorites list
        public async Task AddToFavoritesAsync(string recipeId)
        {
            if (!_authService.IsAuthenticated)
            {
                throw new Exception("User is not authenticated");
            }

            var user = _authService.CurrentUser;
            if (user.FavoriteRecipes == null)
            {
                user.FavoriteRecipes = new List<string>();
            }

            if (!user.FavoriteRecipes.Contains(recipeId))
            {
                user.FavoriteRecipes.Add(recipeId);
                await _authService.UpdateUserAsync(user);
            }
        }

        // Removes a recipe from the current user's favorites list
        public async Task RemoveFromFavoritesAsync(string recipeId)
        {
            if (!_authService.IsAuthenticated)
            {
                throw new Exception("User is not authenticated");
            }

            var user = _authService.CurrentUser;
            if (user.FavoriteRecipes != null && user.FavoriteRecipes.Contains(recipeId))
            {
                user.FavoriteRecipes.Remove(recipeId);
                await _authService.UpdateUserAsync(user);
            }
        }

        // Searches recipes by title using the RecipeSearchService
        public async Task<List<RecipeModel>> SearchByTitleAsync(string query)
        {
            var allRecipes = await GetAllRecipesAsync();
            return _searchService.SearchByTitle(allRecipes, query);
        }

        // Filters recipes by ingredient names using the RecipeSearchService
        public async Task<List<RecipeModel>> FilterByIngredientNamesAsync(List<string> names)
        {
            var allRecipes = await GetAllRecipesAsync();
            return _searchService.FilterByIngredients(allRecipes, names);
        }
    }
}
