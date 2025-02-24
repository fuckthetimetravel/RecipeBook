using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace RecipeBook.Models
{


    /// <summary>
    /// Универсальный конвертер для Firebase Firestore
    /// </summary>
    public static class FirestoreEntity
    {
        /// <summary>
        /// Преобразование данных Firestore в строку
        /// </summary>
        public static string GetStringValue(JsonElement element)
        {
            return element.TryGetProperty("stringValue", out var value) ? value.GetString() : string.Empty;
        }

        /// <summary>
        /// Преобразование массива данных Firestore в список строк
        /// </summary>
        public static List<string> GetStringArray(JsonElement element)
        {
            if (element.TryGetProperty("arrayValue", out var array) && array.TryGetProperty("values", out var values))
            {
                return values.EnumerateArray().Select(GetStringValue).ToList();
            }
            return new List<string>();
        }

        /// <summary>
        /// Преобразование Firestore JSON в объект Recipe
        /// </summary>
        public static Recipe ParseRecipe(JsonElement element)
        {
            return new Recipe
            {
                Id = GetStringValue(element.GetProperty("name")),
                Title = GetStringValue(element.GetProperty("fields").GetProperty("title")),
                Description = GetStringValue(element.GetProperty("fields").GetProperty("description")),
                Ingredients = ParseIngredients(element.GetProperty("fields").GetProperty("ingredients")),
                Steps = ParseSteps(element.GetProperty("fields").GetProperty("steps")),
                AuthorId = GetStringValue(element.GetProperty("fields").GetProperty("authorId"))
            };
        }

        private static List<Ingredient> ParseIngredients(JsonElement element)
        {
            return element.GetProperty("arrayValue").GetProperty("values").EnumerateArray()
                .Select(ParseIngredient).ToList();
        }

        private static Ingredient ParseIngredient(JsonElement element)
        {
            var fields = element.GetProperty("mapValue").GetProperty("fields");
            return new Ingredient
            {
                Name = GetStringValue(fields.GetProperty("name")),
                Quantity = fields.GetProperty("quantity").GetProperty("doubleValue").GetDouble(),
                Unit = GetStringValue(fields.GetProperty("unit"))
            };
        }

        private static List<RecipeStep> ParseSteps(JsonElement element)
        {
            return element.GetProperty("arrayValue").GetProperty("values").EnumerateArray()
                .Select(ParseStep).ToList();
        }

        private static RecipeStep ParseStep(JsonElement element)
        {
            var fields = element.GetProperty("mapValue").GetProperty("fields");
            return new RecipeStep
            {
                Text = GetStringValue(fields.GetProperty("text")),
                ImageUrl = GetStringValue(fields.GetProperty("imageUrl"))
            };
        }
    }

}
