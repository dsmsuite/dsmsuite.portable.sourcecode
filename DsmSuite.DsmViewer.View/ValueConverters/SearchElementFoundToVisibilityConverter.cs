using Avalonia.Data.Converters;
using DsmSuite.DsmViewer.Model.Interfaces;
using System;
using System.Globalization;

namespace DsmSuite.DsmViewer.View.ValueConverters
{
    public class SearchElementFoundToVisibilityConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value != null)
            {
                IDsmElement foundElement = (IDsmElement)value;
                return (foundElement != null);
            }
            else
            {
                return false;
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
