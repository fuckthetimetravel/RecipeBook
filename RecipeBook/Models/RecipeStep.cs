using System.Text.Json.Serialization;

namespace RecipeBook.Models
{
    // Model representing a step in a recipe.
    public class RecipeStep
    {
        // Description of the step.
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
