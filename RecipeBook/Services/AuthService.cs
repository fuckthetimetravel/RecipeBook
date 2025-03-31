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
        private readonly HttpClient _httpClient;
        private const string AUTH_TOKEN_KEY = "auth_token";
        private const string USER_DATA_KEY = "user_data";

        public bool IsAuthenticated => !string.IsNullOrEmpty(AuthToken) && CurrentUser != null;
        public string AuthToken { get; private set; }
        public User CurrentUser { get; private set; }

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            LoadSavedAuthDataAsync().ConfigureAwait(false);
        }

        public AuthService() { }

        public async Task LoadSavedAuthDataAsync()
        {
            try
            {
                AuthToken = await SecureStorage.GetAsync(AUTH_TOKEN_KEY);

                if (!string.IsNullOrEmpty(AuthToken))
                {
                    string userData = await SecureStorage.GetAsync(USER_DATA_KEY);
                    if (!string.IsNullOrEmpty(userData))
                    {
                        CurrentUser = JsonSerializer.Deserialize<User>(userData);
                    }

                    try
                    {
                        if (CurrentUser != null)
                        {
                            await GetUserAsync(CurrentUser.Id); // Проверка валидности токена
                        }
                    }
                    catch
                    {
                        await ClearSavedAuthDataAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading saved auth data: {ex.Message}");
                await ClearSavedAuthDataAsync();
            }
        }

        private async Task SaveAuthDataAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(AuthToken) && CurrentUser != null)
                {
                    await SecureStorage.SetAsync(AUTH_TOKEN_KEY, AuthToken);
                    var userData = JsonSerializer.Serialize(CurrentUser);
                    await SecureStorage.SetAsync(USER_DATA_KEY, userData);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving auth data: {ex.Message}");
            }
        }

        private async Task ClearSavedAuthDataAsync()
        {
            try
            {
                SecureStorage.Remove(AUTH_TOKEN_KEY);
                SecureStorage.Remove(USER_DATA_KEY);
                AuthToken = null;
                CurrentUser = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing auth data: {ex.Message}");
            }
        }

        public async Task<User> SignInAsync(string email, string password)
        {
            var url = FirebaseConfig.FirebaseConfig.GetSignInUrl();

            var requestData = new
            {
                email,
                password,
                returnSecureToken = true
            };

            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to sign in: {responseString}");

            var authResult = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseString);

            if (authResult.TryGetValue("idToken", out var idToken) &&
                authResult.TryGetValue("localId", out var localId))
            {
                AuthToken = idToken.GetString();
                var userId = localId.GetString();

                CurrentUser = await GetUserAsync(userId);
                await SaveAuthDataAsync();

                return CurrentUser;
            }

            throw new Exception("Failed to parse authentication response");
        }

        public async Task<User> SignUpAsync(string email, string password, string firstName, string lastName, string profileImageBase64)
        {
            var url = FirebaseConfig.FirebaseConfig.GetSignUpUrl();

            var requestData = new
            {
                email,
                password,
                returnSecureToken = true
            };

            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to sign up: {responseString}");

            var authResult = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseString);

            if (authResult.TryGetValue("idToken", out var idToken) &&
                authResult.TryGetValue("localId", out var localId))
            {
                AuthToken = idToken.GetString();
                var userId = localId.GetString();

                var user = new User
                {
                    Id = userId,
                    Email = email,
                    FirstName = firstName ?? string.Empty,
                    LastName = lastName ?? string.Empty,
                    ProfileImageBase64 = profileImageBase64 ?? string.Empty,
                    FavoriteRecipes = new List<string>()
                };

                await CreateUserAsync(user);
                CurrentUser = user;
                await SaveAuthDataAsync();

                return user;
            }

            throw new Exception("Failed to parse authentication response");
        }

        public async Task SignOutAsync()
        {
            AuthToken = null;
            CurrentUser = null;
            await ClearSavedAuthDataAsync();
        }

        private async Task CreateUserAsync(User user)
        {
            var url = $"{FirebaseConfig.FirebaseConfig.GetUserUrl(user.Id)}?auth={AuthToken}";

            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to create user: {responseString}");
        }

        public async Task<User> GetUserAsync(string userId)
        {
            var url = $"{FirebaseConfig.FirebaseConfig.GetUserUrl(userId)}?auth={AuthToken}";

            var response = await _httpClient.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to get user: {responseString}");

            return JsonSerializer.Deserialize<User>(responseString);
        }

        public async Task UpdateUserAsync(User user)
        {
            var url = $"{FirebaseConfig.FirebaseConfig.GetUserUrl(user.Id)}?auth={AuthToken}";

            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to update user: {responseString}");

            CurrentUser = user;
            await SaveAuthDataAsync();
        }
    }
}
