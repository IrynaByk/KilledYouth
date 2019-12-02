using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HypertensionControlUI.Views.Converters
{
    public class ZeroToEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var doubleValue = System.Convert.ToDouble(value);
            return doubleValue == 0.0 ? "" : doubleValue.ToString( CultureInfo.InvariantCulture );
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var convertBack = System.Convert.ChangeType( "".Equals( value ) ? 0 : value, targetType, CultureInfo.InvariantCulture );
                return convertBack;
            }
            catch
            {
                return 0;
            }
        }
    }
}
