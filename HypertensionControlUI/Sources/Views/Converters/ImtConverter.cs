using System;
using System.Globalization;
using System.Windows.Data;

namespace HypertensionControlUI.Views.Converters
{
    public class ImtConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
                throw new ArgumentException("Invalid number of parameters");
            try
            {
                var height = System.Convert.ToDouble( values[0] );
                var weight = System.Convert.ToDouble( values[1] );
                return (weight/height/height).ToString( CultureInfo.CurrentCulture );
            }
            catch (FormatException)
            {
                return "";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}