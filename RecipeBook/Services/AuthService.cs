using RecipeBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RecipeBook.Services
{
    public static class AuthService
    {
        private const string ApiKey = "AIzaSyBRLxAAT7_j2cjY58LrclZ3z9bUo9OU8_c";
        private const string DatabaseUrl = "https://recipebook-e2874-default-rtdb.europe-west1.firebasedatabase.app/";

        public static string IdToken { get; set; }
        public static string LocalId { get; set; }
        public static string Email { get; set; }
        public static string FirstName { get; set; }
        public static string LastName { get; set; }
        public static string BirthDate { get; private set; }

        public static async Task<bool> RegisterUser(string email, string password, string firstName, string lastName, string birthDate)
        {
            var signUpResponse = await SendSignUpRequest(email, password);
            if (signUpResponse == null)
            {
                return false;
            }

            IdToken = signUpResponse.idToken;
            LocalId = signUpResponse.localId;
            Email = signUpResponse.email;

            var userSaved = await SaveUserData(firstName, lastName, birthDate);
            if (!userSaved)
            {
                return false;
            }

            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;

            return true;
        }

        public static async Task<bool> LoginUser(string email, string password)
        {
            var signInResponse = await SendSignInRequest(email, password);
            if (signInResponse == null)
            {
                return false;
            }

            IdToken = signInResponse.idToken;
            LocalId = signInResponse.localId;
            Email = signInResponse.email;

            var userData = await GetUserData();
            if (userData != null)
            {
                FirstName = userData.firstName;
                LastName = userData.lastName;
            }

            return true;
        }

        private static async Task<FirebaseSignUpResponse> SendSignUpRequest(string email, string password)
        {
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={ApiKey}";
            var body = new { email, password, returnSecureToken = true };

            var response = await PostJson(url, body);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<FirebaseSignUpResponse>(responseBody);
        }

        private static async Task<FirebaseSignUpResponse> SendSignInRequest(string email, string password)
        {
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={ApiKey}";
            var body = new { email, password, returnSecureToken = true };

            var response = await PostJson(url, body);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<FirebaseSignUpResponse>(responseBody);
        }

        private static async Task<bool> SaveUserData(string firstName, string lastName, string birthDate)
        {
            var url = $"{DatabaseUrl}/users/{LocalId}.json?auth={IdToken}";
            var body = new { firstName, lastName, birthDate };

            var response = await PutJson(url, body);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error saving user data: {error}");
            }

            return response.IsSuccessStatusCode;
        }

        private static async Task<UserData> GetUserData()
        {
            var url = $"{DatabaseUrl}/users/{LocalId}.json?auth={IdToken}";

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error fetching user data: {responseBody}");
                return null;
            }

            var userData = JsonSerializer.Deserialize<UserData>(responseBody);

            // ✅ Сохраняем дату рождения в AuthService
            if (userData != null)
            {
                FirstName = userData.firstName;
                LastName = userData.lastName;
                BirthDate = userData.birthDate;
            }

            return userData;
        }

        private static async Task<HttpResponseMessage> PostJson(string url, object body)
        {
            using var httpClient = new HttpClient();
            var jsonContent = JsonSerializer.Serialize(body);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            return await httpClient.PostAsync(url, content);
        }

        private static async Task<HttpResponseMessage> PutJson(string url, object body)
        {
            using var httpClient = new HttpClient();
            var jsonContent = JsonSerializer.Serialize(body);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            return await httpClient.PutAsync(url, content);
        }

        public static async Task<bool> PostRecipeAsync(string title, string description, string ingredients, string steps)
        {
            if (string.IsNullOrEmpty(IdToken) || string.IsNullOrEmpty(LocalId))
            {
                Console.WriteLine("User is not authenticated.");
                return false;
            }

            var url = $"{DatabaseUrl}/recipes/{LocalId}.json?auth={IdToken}";

            var newRecipe = new RecipeModel
            {
                Title = title,
                Description = description,
                Ingredients = ingredients,
                Steps = steps,
                CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            };

            var response = await PostJson(url, newRecipe);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error posting recipe: {error}");
            }

            return response.IsSuccessStatusCode;
        }

        public static async Task<List<RecipeModel>> GetRecipesAsync()
        {
            if (string.IsNullOrEmpty(IdToken) || string.IsNullOrEmpty(LocalId))
            {
                Console.WriteLine("User is not authenticated.");
                return new List<RecipeModel>();
            }

            var url = $"{DatabaseUrl}/recipes/{LocalId}.json?auth={IdToken}";

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error fetching recipes: {responseBody}");
                return new List<RecipeModel>();
            }

            var recipesDict = JsonSerializer.Deserialize<Dictionary<string, RecipeModel>>(responseBody);
            return recipesDict?.Values.ToList() ?? new List<RecipeModel>();
        }


    }

    public class FirebaseSignUpResponse
    {
        public string idToken { get; set; }
        public string email { get; set; }
        public string refreshToken { get; set; }
        public string expiresIn { get; set; }
        public string localId { get; set; }
    }

    public class UserData
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string birthDate { get; set; }
    }
}
