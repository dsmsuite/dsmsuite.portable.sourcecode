using System;
using System.Globalization;
using Avalonia.Data.Converters;
using DsmSuite.DsmViewer.ViewModel.Common;
using DsmSuite.DsmViewer.ViewModel.Main;

namespace DsmSuite.DsmViewer.View.ValueConverters
{
    public class SearchIndicatorModeToVisibleConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value != null)
            {
                IndicatorViewMode viewMode = (IndicatorViewMode)value;
                return viewMode == IndicatorViewMode.Search;
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
