using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RecipeBook.Models
{
    /// <summary>
    /// Модель рецепта
    /// </summary>
    public class RecipeModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString(); // Генерируем уникальный ID

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("ingredients")]
        public List<Ingredient> Ingredients { get; set; } = new();

        [JsonPropertyName("steps")]
        public List<RecipeStep> Steps { get; set; } = new();

        [JsonPropertyName("authorId")]
        public string AuthorId { get; set; } // ID пользователя, который добавил рецепт

        [JsonPropertyName("imageBase64")]
        public string ImageBase64 { get; set; } // Изображение блюда в формате Base64
    }
}