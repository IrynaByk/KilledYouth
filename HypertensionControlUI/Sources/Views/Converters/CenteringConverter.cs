using System;
using System.Globalization;
using System.Windows.Data;

namespace HypertensionControlUI.Views.Converters
{
    public class CenteringConverter : IValueConverter
    {
        public static readonly CenteringConverter Instance = new CenteringConverter();

        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            return -(System.Convert.ToDouble( value ) / 2);
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}