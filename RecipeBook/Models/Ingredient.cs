using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RecipeBook.Models
{
    /// <summary>
    /// Модель ингредиента
    /// </summary>
    public class Ingredient
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("quantity")]
        public string Quantity { get; set; }
    }
}