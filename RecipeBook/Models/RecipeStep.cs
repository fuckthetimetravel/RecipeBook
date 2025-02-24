using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBook.Models
{
    /// <summary>
    /// Шаг приготовления в рецепте
    /// </summary>
    public class RecipeStep
    {
        public string Text { get; set; } // Описание шага
        public string ImageUrl { get; set; } // Ссылка на изображение шага

        /// <summary>
        /// Конвертация в формат Firestore
        /// </summary>
        public object ToFirestoreFormat()
        {
            return new
            {
                mapValue = new
                {
                    fields = new
                    {
                        text = new { stringValue = Text },
                        imageUrl = new { stringValue = ImageUrl }
                    }
                }
            };
        }
    }

}
