using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RecipeBook.Models
{
    public class User
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("favoriteRecipes")]
        public List<string> FavoriteRecipes { get; set; }

        // Helper property to get full name
        [JsonIgnore]
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}