using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RecipeBook.Models
{
    // Model representing a recipe.
    public class RecipeModel
    {
        // Unique identifier for the recipe, generated using a new GUID.
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // Title of the recipe.
        [JsonPropertyName("title")]
        public string Title { get; set; }

        // Description of the recipe.
        [JsonPropertyName("description")]
        public string Description { get; set; }

        // List of ingredients used in the recipe.
        [JsonPropertyName("ingredients")]
        public List<Ingredient> Ingredients { get; set; } = new();

        // List of steps for preparing the recipe.
        [JsonPropertyName("steps")]
        public List<RecipeStep> Steps { get; set; } = new();

        // Identifier of the user who created the recipe.
        [JsonPropertyName("authorId")]
        public string AuthorId { get; set; }

        // Image of the recipe in Base64 format.
        [JsonPropertyName("imageBase64")]
        public string ImageBase64 { get; set; }

        // Location where the recipe is saved.
        [JsonPropertyName("location")]
        public string Location { get; set; }
    }
}
