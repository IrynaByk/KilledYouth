using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace HypertensionControlUI.Views.Converters
{
    public class NonNullConverter : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            return value is null ? "" : value;
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}