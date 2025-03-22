using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RecipeBook.Models
{
    /// <summary>
    /// Модель пользователя
    /// </summary>
    public class User
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("favoriteRecipes")]
        public List<string> FavoriteRecipes { get; set; } = new(); // Список избранных рецептов
    }
}