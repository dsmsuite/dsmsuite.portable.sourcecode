using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace DsmSuite.DsmViewer.View.ValueConverters
{
    public class MatrixSizeToVisbilityConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value != null)
            {
                int matrixSize = (int)value;
                return matrixSize > 0;
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
