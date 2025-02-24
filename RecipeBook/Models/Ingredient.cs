using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBook.Models
{
    /// <summary>
    /// Модель ингредиента
    /// </summary>
    public class Ingredient
    {
        public string Name { get; set; }
        public double Quantity { get; set; }
        public string Unit { get; set; } // Например: "grams", "ml", "pieces"

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
                        name = new { stringValue = Name },
                        quantity = new { doubleValue = Quantity },
                        unit = new { stringValue = Unit }
                    }
                }
            };
        }
    }

}
