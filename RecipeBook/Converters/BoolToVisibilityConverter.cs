using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace RecipeBook.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                // If parameter is provided, invert the result
                if (parameter is string strParam && strParam.ToLower() == "true")
                {
                    return !boolValue;
                }
                return boolValue;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}