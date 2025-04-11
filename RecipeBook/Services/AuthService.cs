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
        // HttpClient instance for HTTP requests
        private readonly HttpClient _httpClient;
        // Keys for secure storage of authentication token and user data
        private const string AUTH_TOKEN_KEY = "auth_token";
        private const string USER_DATA_KEY = "user_data";

        // Indicates whether a user is currently authenticated
        public bool IsAuthenticated => !string.IsNullOrEmpty(AuthToken) && CurrentUser != null;
        // Stores the current authentication token
        public string AuthToken { get; private set; }
        // Stores the current user information
        public User CurrentUser { get; private set; }

        // Constructor that initializes the HttpClient and loads saved authentication data
        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            LoadSavedAuthDataAsync().ConfigureAwait(false);
        }

        // Parameterless constructor
        public AuthService() { }

        // Loads saved authentication token and user data from secure storage
        public async Task LoadSavedAuthDataAsync()
        {
            try
            {
                // Retrieve stored authentication token
                AuthToken = await SecureStorage.GetAsync(AUTH_TOKEN_KEY);

                if (!string.IsNullOrEmpty(AuthToken))
                {
                    // Retrieve stored user data if available
                    string userData = await SecureStorage.GetAsync(USER_DATA_KEY);
                    if (!string.IsNullOrEmpty(userData))
                    {
                        CurrentUser = JsonSerializer.Deserialize<User>(userData);
                    }

                    try
                    {
                        if (CurrentUser != null)
                        {
                            // Check token validity by fetching the current user from the server
                            await GetUserAsync(CurrentUser.Id);
                        }
                    }
                    catch
                    {
                        // Clear saved authentication data if token is no longer valid
                        await ClearSavedAuthDataAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log any exception and clear saved authentication data
                System.Diagnostics.Debug.WriteLine($"Error loading saved auth data: {ex.Message}");
                await ClearSavedAuthDataAsync();
            }
        }

        // Saves the current authentication token and user data to secure storage
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
                // Log any exception encountered during saving
                System.Diagnostics.Debug.WriteLine($"Error saving auth data: {ex.Message}");
            }
        }

        // Clears the saved authentication token and user data from secure storage
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
                // Log any exception encountered during clearing
                System.Diagnostics.Debug.WriteLine($"Error clearing auth data: {ex.Message}");
            }
        }

        // Signs in a user using the provided email and password via Firebase authentication
        public async Task<User> SignInAsync(string email, string password)
        {
            // Get the Firebase sign-in URL
            var url = FirebaseConfig.FirebaseConfig.GetSignInUrl();

            // Create the request payload for sign-in
            var requestData = new
            {
                email,
                password,
                returnSecureToken = true
            };

            // Send a POST request with the sign-in data
            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to sign in: {responseString}");

            // Deserialize the authentication response
            var authResult = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseString);

            if (authResult.TryGetValue("idToken", out var idToken) &&
                authResult.TryGetValue("localId", out var localId))
            {
                // Set the authentication token and obtain the user ID
                AuthToken = idToken.GetString();
                var userId = localId.GetString();

                // Retrieve user data from the server and save authentication data locally
                CurrentUser = await GetUserAsync(userId);
                await SaveAuthDataAsync();

                return CurrentUser;
            }

            throw new Exception("Failed to parse authentication response");
        }

        // Signs up a new user using Firebase authentication and creates a user record
        public async Task<User> SignUpAsync(string email, string password, string firstName, string lastName, string profileImageBase64)
        {
            // Get the Firebase sign-up URL
            var url = FirebaseConfig.FirebaseConfig.GetSignUpUrl();

            // Create the request payload for sign-up
            var requestData = new
            {
                email,
                password,
                returnSecureToken = true
            };

            // Send a POST request with the sign-up data
            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to sign up: {responseString}");

            // Deserialize the authentication response
            var authResult = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseString);

            if (authResult.TryGetValue("idToken", out var idToken) &&
                authResult.TryGetValue("localId", out var localId))
            {
                // Set the authentication token and obtain the user ID
                AuthToken = idToken.GetString();
                var userId = localId.GetString();

                // Create a new user object with the provided details
                var user = new User
                {
                    Id = userId,
                    Email = email,
                    FirstName = firstName ?? string.Empty,
                    LastName = lastName ?? string.Empty,
                    ProfileImageBase64 = profileImageBase64 ?? string.Empty,
                    FavoriteRecipes = new List<string>()
                };

                // Create a new user record in the database
                await CreateUserAsync(user);
                CurrentUser = user;
                await SaveAuthDataAsync();

                return user;
            }

            throw new Exception("Failed to parse authentication response");
        }

        // Signs out the current user and clears authentication data
        public async Task SignOutAsync()
        {
            AuthToken = null;
            CurrentUser = null;
            await ClearSavedAuthDataAsync();
        }

        // Creates a new user record in the database using Firebase
        private async Task CreateUserAsync(User user)
        {
            // Build the URL for creating the user record with authentication token
            var url = $"{FirebaseConfig.FirebaseConfig.GetUserUrl(user.Id)}?auth={AuthToken}";

            // Serialize the user object and send a PUT request to create the record
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to create user: {responseString}");
        }

        // Retrieves a user record from the database based on the user ID
        public async Task<User> GetUserAsync(string userId)
        {
            // Build the URL for retrieving the user record with authentication token
            var url = $"{FirebaseConfig.FirebaseConfig.GetUserUrl(userId)}?auth={AuthToken}";

            var response = await _httpClient.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to get user: {responseString}");

            return JsonSerializer.Deserialize<User>(responseString);
        }

        // Updates an existing user record in the database
        public async Task UpdateUserAsync(User user)
        {
            // Build the URL for updating the user record with authentication token
            var url = $"{FirebaseConfig.FirebaseConfig.GetUserUrl(user.Id)}?auth={AuthToken}";

            // Serialize the updated user object and send a PUT request
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to update user: {responseString}");

            // Update the current user and save the new authentication data locally
            CurrentUser = user;
            await SaveAuthDataAsync();
        }
    }
}
