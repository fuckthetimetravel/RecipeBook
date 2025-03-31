using RecipeBook.Models;
using System.Globalization;

namespace RecipeBook.Converters
{
    public class IndexToStepNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return "Step:";

            if (parameter is IList<RecipeStep> steps)
            {
                int index = steps.IndexOf((RecipeStep)value);
                return $"Step {index + 1}:";
            }

            return "Step:";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}