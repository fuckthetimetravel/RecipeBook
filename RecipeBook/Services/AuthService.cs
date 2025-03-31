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
            // Try to load saved authentication data when service is created
            LoadSavedAuthDataAsync().ConfigureAwait(false);
        }

        public async Task LoadSavedAuthDataAsync()
        {
            try
            {
                // Get saved auth token
                AuthToken = await SecureStorage.GetAsync(AUTH_TOKEN_KEY);

                if (!string.IsNullOrEmpty(AuthToken))
                {
                    // Get saved user data
                    string userData = await SecureStorage.GetAsync(USER_DATA_KEY);
                    if (!string.IsNullOrEmpty(userData))
                    {
                        CurrentUser = JsonSerializer.Deserialize<User>(userData);
                    }

                    // Validate the token by making a request
                    if (CurrentUser != null)
                    {
                        // Optional: Validate token with a lightweight API call
                        // If validation fails, clear the saved data
                        try
                        {
                            await GetUserAsync(CurrentUser.Id);
                        }
                        catch
                        {
                            await ClearSavedAuthDataAsync();
                        }
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
                    string userData = JsonSerializer.Serialize(CurrentUser);
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

            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to sign in: {responseString}");
            }

            var authResult = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseString);

            if (authResult.TryGetValue("idToken", out var idToken))
            {
                AuthToken = idToken.GetString();

                if (authResult.TryGetValue("localId", out var localId))
                {
                    var userId = localId.GetString();
                    CurrentUser = await GetUserAsync(userId);

                    // Save authentication data for persistent login
                    await SaveAuthDataAsync();

                    return CurrentUser;
                }
            }

            throw new Exception("Failed to parse authentication response");
        }

        public async Task<User> SignUpAsync(string email, string password, string firstName, string lastName)
        {
            var url = FirebaseConfig.FirebaseConfig.GetSignUpUrl();

            var requestData = new
            {
                email,
                password,
                returnSecureToken = true
            };

            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to sign up: {responseString}");
            }

            var authResult = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseString);

            if (authResult.TryGetValue("idToken", out var idToken) &&
                authResult.TryGetValue("localId", out var localId))
            {
                AuthToken = idToken.GetString();
                var userId = localId.GetString();

                // Create user profile
                var user = new User
                {
                    Id = userId,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    FavoriteRecipes = new List<string>()
                };

                await CreateUserAsync(user);
                CurrentUser = user;

                // Save authentication data for persistent login
                await SaveAuthDataAsync();

                return user;
            }

            throw new Exception("Failed to parse authentication response");
        }

        public async Task SignOutAsync()
        {
            AuthToken = null;
            CurrentUser = null;

            // Clear saved authentication data
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
            {
                throw new Exception($"Failed to create user: {responseString}");
            }
        }

        public async Task<User> GetUserAsync(string userId)
        {
            var url = $"{FirebaseConfig.FirebaseConfig.GetUserUrl(userId)}?auth={AuthToken}";

            var response = await _httpClient.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get user: {responseString}");
            }

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
            {
                throw new Exception($"Failed to update user: {responseString}");
            }

            // Update current user
            CurrentUser = user;

            // Update saved user data
            await SaveAuthDataAsync();
        }
    }
}