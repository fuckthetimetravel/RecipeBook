using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RecipeBook.Models
{
    // Represents a user within the application.
    public class User
    {
        // Unique identifier for the user.
        [JsonPropertyName("id")]
        public string Id { get; set; }

        // User's email address.
        [JsonPropertyName("email")]
        public string Email { get; set; }

        // User's first name.
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        // User's last name.
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        // Collection of recipe IDs that the user has marked as favorites.
        [JsonPropertyName("favoriteRecipes")]
        public List<string> FavoriteRecipes { get; set; }

        // Profile image of the user in Base64 format.
        [JsonPropertyName("profileImageBase64")]
        public string ProfileImageBase64 { get; set; }

        // Returns the full name by concatenating the first and last names.
        [JsonIgnore]
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
