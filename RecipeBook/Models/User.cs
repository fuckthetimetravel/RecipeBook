using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBook.Models
{
    /// <summary>
    /// Модель пользователя
    /// </summary>
    public class User
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public List<string> FavoriteRecipes { get; set; } = new(); // Список избранных рецептов

        /// <summary>
        /// Конвертация в формат Firestore
        /// </summary>
        public Dictionary<string, object> ToFirestoreFormat()
        {
            return new Dictionary<string, object>
        {
            { "email", new { stringValue = Email } },
            { "favoriteRecipes", new { arrayValue = new { values = FavoriteRecipes.Select(r => new { stringValue = r }).ToList() } } }
        };
        }
    }

}
