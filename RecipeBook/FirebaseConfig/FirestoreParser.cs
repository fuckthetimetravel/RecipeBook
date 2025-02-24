using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using RecipeBook.Models;


namespace RecipeBook.FirebaseConfig
{

    public static class FirestoreParser
    {
        public static List<Recipe> ParseRecipes(string json)
        {
            var result = JsonSerializer.Deserialize<FirestoreDocumentList>(json);
            return result?.Documents.Select(d => d.ToRecipe()).ToList() ?? new List<Recipe>();
        }
    }

    public class FirestoreDocumentList
    {
        public List<FirestoreDocument> Documents { get; set; }
    }

    public class FirestoreDocument
    {
        public string Name { get; set; }
        public FirestoreFields Fields { get; set; }

        public Recipe ToRecipe()
        {
            return new Recipe
            {
                Id = Name.Split('/').Last(),
                Title = Fields.Title?.StringValue,
                Description = Fields.Description?.StringValue
            };
        }
    }

    public class FirestoreFields
    {
        public FirestoreValue Title { get; set; }
        public FirestoreValue Description { get; set; }
    }

    public class FirestoreValue
    {
        public string StringValue { get; set; }
    }

}
