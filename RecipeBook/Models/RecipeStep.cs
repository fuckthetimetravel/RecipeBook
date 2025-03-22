using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RecipeBook.Models
{
    /// <summary>
    /// Шаг приготовления в рецепте
    /// </summary>
    public class RecipeStep
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } // Описание шага
    }
}