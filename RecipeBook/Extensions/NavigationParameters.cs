using System;
using Microsoft.Maui.Controls;
using System.Collections.Generic;

namespace RecipeBook.Extensions
{
    public static class NavigationExtensions
    {
        public static string? GetQueryParameter(this ShellNavigationState state, string parameterName)
        {
            if (state == null || state.Location == null)
                return null;

            var uriString = state.Location.ToString();
            var uri = new Uri(uriString);

            var query = uri.Query; // Это часть после ?

            if (string.IsNullOrEmpty(query))
                return null;

            // Убираем ? и разбираем параметры
            var parameters = ParseQueryString(query.TrimStart('?'));
            return parameters.TryGetValue(parameterName, out var value) ? value : null;
        }

        // Самописный парсер query string
        private static Dictionary<string, string> ParseQueryString(string query)
        {
            var result = new Dictionary<string, string>();
            var pairs = query.Split('&', StringSplitOptions.RemoveEmptyEntries);

            foreach (var pair in pairs)
            {
                var parts = pair.Split('=', 2);
                var key = Uri.UnescapeDataString(parts[0]);
                var value = parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : "";
                result[key] = value;
            }

            return result;
        }
    }
}
