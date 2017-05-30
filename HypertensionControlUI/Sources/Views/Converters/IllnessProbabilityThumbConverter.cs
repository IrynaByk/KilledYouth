using System;
using System.Globalization;
using System.Windows.Data;

namespace HypertensionControlUI.Views.Converters
{
    public class IllnessProbabilityThumbConverter : IValueConverter
    {
        public static readonly IllnessProbabilityThumbConverter Instance = new IllnessProbabilityThumbConverter();

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