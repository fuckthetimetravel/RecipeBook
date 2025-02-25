using Microsoft.Maui.Storage;
using RecipeBook.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RecipeBook.FirebaseConfig;

/// <summary>
/// Сервис аутентификации через Firebase
/// </summary>
public class FirestoreAuthService
{
    private readonly HttpClient _httpClient;

    public FirestoreAuthService()
    {
        _httpClient = FirebaseConfig.GetHttpClient();
    }

    /// <summary>
    /// Регистрация нового пользователя
    /// </summary>
    public async Task<string> RegisterUserAsync(string email, string password, UserProfile profile)
    {
        string url = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={FirebaseConfig.ApiKey}";

        var payload = new { email, password, returnSecureToken = true };
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        var result = await response.Content.ReadAsStringAsync();

        var data = JsonSerializer.Deserialize<FirebaseAuthResponse>(result);
        if (data?.IdToken == null)
            throw new Exception("Registration failed");

        // Сохранение данных профиля в Realtime Database
        await SaveUserProfileAsync(data.LocalId, profile);

        return data.IdToken;
    }

    private async Task SaveUserProfileAsync(string userId, UserProfile profile)
    {
        string url = $"{FirebaseConfig.RealtimeDatabaseUrl}/users/{userId}.json";

        var json = JsonSerializer.Serialize(profile);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync(url, content);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to save user profile: {error}");
        }
    }

    /// <summary>
    /// Авторизация пользователя
    /// </summary>
    public async Task<string> LoginUserAsync(string email, string password)
    {
        string url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={FirebaseConfig.ApiKey}";

        var payload = new { email, password, returnSecureToken = true };
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        var result = await response.Content.ReadAsStringAsync();

        var data = JsonSerializer.Deserialize<FirebaseAuthResponse>(result);
        return data?.IdToken ?? throw new Exception("Login failed");
    }

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
    /// Получение данных текущего пользователя
    /// </summary>
    public async Task<User> GetUserProfileAsync(string idToken)
    {
        string url = $"https://identitytoolkit.googleapis.com/v1/accounts:lookup?key={FirebaseConfig.ApiKey}";

        var payload = new { idToken };
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        var result = await response.Content.ReadAsStringAsync();

        var data = JsonSerializer.Deserialize<FirebaseUserResponse>(result);
        var userInfo = data?.Users?.FirstOrDefault();

        return userInfo != null ? new User { Id = userInfo.LocalId, Email = userInfo.Email } : null;
    }

    /// <summary>
    /// Выход из аккаунта
    /// </summary>
    public async Task LogoutAsync()
    {
        await SecureStorage.SetAsync("auth_token", string.Empty);
    }
}

/// <summary>
/// Ответ Firebase при аутентификации
/// </summary>
public class FirebaseAuthResponse
{
    [JsonPropertyName("idToken")]
    public string IdToken { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; }

    [JsonPropertyName("expiresIn")]
    public string ExpiresIn { get; set; }

    [JsonPropertyName("localId")]
    public string LocalId { get; set; }
}


/// <summary>
/// Ответ Firebase при запросе профиля
/// </summary>
public class FirebaseUserResponse
{
    public List<FirebaseUserInfo> Users { get; set; }
}

public class FirebaseUserInfo
{
    public string LocalId { get; set; }
    public string Email { get; set; }
}


public class UserProfile
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string BirthDate { get; set; } // Дата рождения как строка "yyyy-MM-dd"
}