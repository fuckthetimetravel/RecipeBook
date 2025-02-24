using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace RecipeBook.Models
{


    /// <summary>
    /// Модель рецепта
    /// </summary>
    public class Recipe
    {
        public string Id { get; set; } = Guid.NewGuid().ToString(); // Генерируем уникальный ID
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Ingredient> Ingredients { get; set; } = new();
        public List<RecipeStep> Steps { get; set; } = new();
        public string AuthorId { get; set; } // ID пользователя, который добавил рецепт

        /// <summary>
        /// Конвертация в формат Firestore
        /// </summary>
        public Dictionary<string, object> ToFirestoreFormat()
        {
            return new Dictionary<string, object>
        {
            { "title", new { stringValue = Title } },
            { "description", new { stringValue = Description } },
            { "authorId", new { stringValue = AuthorId } },
            { "ingredients", new { arrayValue = new { values = Ingredients.Select(i => i.ToFirestoreFormat()).ToList() } } },
            { "steps", new { arrayValue = new { values = Steps.Select(s => s.ToFirestoreFormat()).ToList() } } }
        };
        }
    }

}
