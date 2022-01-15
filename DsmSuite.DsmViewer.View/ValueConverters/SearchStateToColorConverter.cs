using Avalonia.Data.Converters;
using Avalonia.Media;
using DsmSuite.DsmViewer.ViewModel.Main;
using System;
using System.Globalization;

namespace DsmSuite.DsmViewer.View.ValueConverters
{
    public class SearchStateToColorConverter : IValueConverter
    {
        public SolidColorBrush NoMatchBrush { get; set; }
        public SolidColorBrush MatchBrush { get; set; }
        public SolidColorBrush MultipleMatchesBrush { get; set; }

        public SearchStateToColorConverter()
        {
            NoMatchBrush = new SolidColorBrush(Colors.Black);
            MatchBrush = new SolidColorBrush(Colors.Black);
            MultipleMatchesBrush = new SolidColorBrush(Colors.Black);
        }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);

            if (value != null)
            {

                SearchState searchState = (SearchState)value;
                switch (searchState)
                {
                    case SearchState.NoInput:
                        brush = NoMatchBrush;
                        break;
                    case SearchState.NoMatch:
                        brush = NoMatchBrush;
                        break;
                    case SearchState.Match:
                        brush = MatchBrush;
                        break;
                    default:
                        brush = NoMatchBrush;
                        break;
                }
            }

            return brush;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
