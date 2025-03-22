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
        public static RecipeModel ParseRecipe(JsonElement element)
        {
            var recipe = new RecipeModel
            {
                Id = GetStringValue(element.GetProperty("name")).Split('/').Last(),
                Title = GetStringValue(element.GetProperty("fields").GetProperty("title")),
                Description = GetStringValue(element.GetProperty("fields").GetProperty("description")),
                AuthorId = GetStringValue(element.GetProperty("fields").GetProperty("authorId"))
            };

            // Парсинг ингредиентов
            if (element.GetProperty("fields").TryGetProperty("ingredients", out var ingredientsElement))
            {
                recipe.Ingredients = ParseIngredients(ingredientsElement);
            }

            // Парсинг шагов
            if (element.GetProperty("fields").TryGetProperty("steps", out var stepsElement))
            {
                recipe.Steps = ParseSteps(stepsElement);
            }

            return recipe;
        }

        /// <summary>
        /// Преобразование Firestore JSON в список ингредиентов
        /// </summary>
        public static List<Ingredient> ParseIngredients(JsonElement element)
        {
            var ingredients = new List<Ingredient>();

            if (element.TryGetProperty("arrayValue", out var array) && array.TryGetProperty("values", out var values))
            {
                foreach (var value in values.EnumerateArray())
                {
                    if (value.TryGetProperty("mapValue", out var mapValue) &&
                        mapValue.TryGetProperty("fields", out var fields))
                    {
                        var ingredient = new Ingredient
                        {
                            Name = GetStringValue(fields.GetProperty("name")),
                            Quantity = GetStringValue(fields.GetProperty("quantity"))
                        };
                        ingredients.Add(ingredient);
                    }
                }
            }

            return ingredients;
        }

        /// <summary>
        /// Преобразование Firestore JSON в список шагов рецепта
        /// </summary>
        public static List<RecipeStep> ParseSteps(JsonElement element)
        {
            var steps = new List<RecipeStep>();

            if (element.TryGetProperty("arrayValue", out var array) && array.TryGetProperty("values", out var values))
            {
                foreach (var value in values.EnumerateArray())
                {
                    if (value.TryGetProperty("mapValue", out var mapValue) &&
                        mapValue.TryGetProperty("fields", out var fields))
                    {
                        var step = new RecipeStep
                        {
                            Text = GetStringValue(fields.GetProperty("text")),
                            //ImageUrl = GetStringValue(fields.GetProperty("imageUrl"))
                        };
                        steps.Add(step);
                    }
                }
            }

            return steps;
        }

        /// <summary>
        /// Преобразование Firestore JSON в объект User
        /// </summary>
        public static User ParseUser(JsonElement element)
        {
            var user = new User
            {
                Id = GetStringValue(element.GetProperty("name")).Split('/').Last(),
                Email = GetStringValue(element.GetProperty("fields").GetProperty("email"))
            };

            // Парсинг избранных рецептов
            if (element.GetProperty("fields").TryGetProperty("favoriteRecipes", out var favoritesElement))
            {
                user.FavoriteRecipes = GetStringArray(favoritesElement);
            }

            return user;
        }
    }
}