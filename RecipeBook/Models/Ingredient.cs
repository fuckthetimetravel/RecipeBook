using System.Text.Json.Serialization;

namespace RecipeBook.Models
{
    // Model representing an ingredient.
    public class Ingredient
    {
        // Name of the ingredient.
        [JsonPropertyName("name")]
        public string Name { get; set; }

        // Quantity of the ingredient.
        [JsonPropertyName("quantity")]
        public string Quantity { get; set; }
    }
}
