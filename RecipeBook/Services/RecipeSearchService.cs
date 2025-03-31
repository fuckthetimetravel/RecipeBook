using RecipeBook.Models;

namespace RecipeBook.Services
{
    public class RecipeSearchService
    {
        /// Поиск рецептов по названию (с поддержкой подстрок и опечаток)
        public List<RecipeModel> SearchByTitle(IEnumerable<RecipeModel> recipes, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<RecipeModel>();

            string input = query.Trim().ToLowerInvariant();

            return recipes
                .Where(recipe =>
                    !string.IsNullOrWhiteSpace(recipe.Title) &&
                    (recipe.Title.ToLowerInvariant().Contains(input) ||
                     LevenshteinDistance(input, recipe.Title.ToLowerInvariant()) <= 2)
                )
                .ToList();
        }

        /// Фильтрация рецептов по ингредиентам (по имени, игнорируя количество)
        public List<RecipeModel> FilterByIngredients(IEnumerable<RecipeModel> recipes, List<string> ingredientNames)
        {
            if (ingredientNames == null || ingredientNames.Count == 0)
                return new List<RecipeModel>();

            return recipes.Where(recipe =>
                recipe.Ingredients != null &&
                ingredientNames.All(input =>
                    recipe.Ingredients.Any(i => IsSimilar(input, i.Name))
                )
            ).ToList();
        }

        // ============================================
        // PRIVATE HELPERS
        // ============================================

        private bool IsSimilar(string input, string actual)
        {
            if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(actual))
                return false;

            input = input.Trim().ToLowerInvariant();
            actual = actual.Trim().ToLowerInvariant();

            if (actual.Contains(input))
                return true;

            int distance = LevenshteinDistance(input, actual);
            return distance <= 2;
        }

        private int LevenshteinDistance(string a, string b)
        {
            int[,] dp = new int[a.Length + 1, b.Length + 1];

            for (int i = 0; i <= a.Length; i++) dp[i, 0] = i;
            for (int j = 0; j <= b.Length; j++) dp[0, j] = j;

            for (int i = 1; i <= a.Length; i++)
            {
                for (int j = 1; j <= b.Length; j++)
                {
                    int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                    dp[i, j] = Math.Min(
                        Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                        dp[i - 1, j - 1] + cost
                    );
                }
            }

            return dp[a.Length, b.Length];
        }
    }
}
