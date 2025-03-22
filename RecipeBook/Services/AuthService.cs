using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RecipeBook.Models;

namespace RecipeBook.Services
{
    public class AuthService
    {
        private HttpClient _httpClient;
        private string _authToken;
        private User _currentUser;

        public User CurrentUser => _currentUser;
        public bool IsAuthenticated => _currentUser != null;
        public string AuthToken => _authToken;

        public AuthService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<User> SignUpAsync(string email, string password)
        {
            var payload = new
            {
                email,
                password,
                returnSecureToken = true
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(FirebaseConfig.FirebaseConfig.GetSignUpUrl(), content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var error = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseString);
                if (error.TryGetValue("error", out var errorDetails) &&
                    errorDetails.TryGetProperty("message", out var errorMessage))
                {
                    throw new Exception($"Failed to sign up: {errorMessage.GetString()}");
                }
                throw new Exception($"Failed to sign up: {responseString}");
            }

            var authResult = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseString);
            var userId = authResult["localId"].GetString();
            _authToken = authResult["idToken"].GetString();

            // Создаем пользователя в Realtime Database
            _currentUser = new User
            {
                Id = userId,
                Email = email,
                FavoriteRecipes = new List<string>()
            };

            await CreateUserInDatabase(_currentUser);

            return _currentUser;
        }

        public async Task<User> SignInAsync(string email, string password)
        {
            var payload = new
            {
                email,
                password,
                returnSecureToken = true
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(FirebaseConfig.FirebaseConfig.GetSignInUrl(), content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var error = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseString);
                if (error.TryGetValue("error", out var errorDetails) &&
                    errorDetails.TryGetProperty("message", out var errorMessage))
                {
                    throw new Exception($"Failed to sign in: {errorMessage.GetString()}");
                }
                throw new Exception($"Failed to sign in: {responseString}");
            }

            var authResult = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseString);
            var userId = authResult["localId"].GetString();
            _authToken = authResult["idToken"].GetString();

            // Получаем данные пользователя из Realtime Database
            _currentUser = await GetUserFromDatabase(userId);

            return _currentUser;
        }

        public void SignOut()
        {
            _currentUser = null;
            _authToken = null;
        }

        private async Task CreateUserInDatabase(User user)
        {
            var url = $"{FirebaseConfig.FirebaseConfig.GetUserUrl(user.Id)}?auth={_authToken}";
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, url);
            request.Content = content;

            var response = await _httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to create user in database: {responseString}");
            }
        }

        private async Task<User> GetUserFromDatabase(string userId)
        {
            var url = $"{FirebaseConfig.FirebaseConfig.GetUserUrl(userId)}?auth={_authToken}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await _httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get user from database: {responseString}");
            }

            var user = JsonSerializer.Deserialize<User>(responseString);
            if (user == null)
            {
                // Если пользователь не найден, создаем новый
                user = new User
                {
                    Id = userId,
                    Email = "unknown@email.com", // Можно получить из токена
                    FavoriteRecipes = new List<string>()
                };
                await CreateUserInDatabase(user);
            }

            // Убедимся, что ID установлен
            user.Id = userId;

            return user;
        }

        public async Task UpdateUserAsync(User user)
        {
            var url = $"{FirebaseConfig.FirebaseConfig.GetUserUrl(user.Id)}?auth={_authToken}";

            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, url);
            request.Content = content;

            var response = await _httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to update user in database: {responseString}");
            }

            _currentUser = user;
        }
    }
}