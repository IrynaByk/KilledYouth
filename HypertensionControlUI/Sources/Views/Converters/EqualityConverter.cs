using System;
using System.Globalization;
using System.Windows.Data;

namespace HypertensionControlUI.Views.Converters
{
    public class EqualityConverter : IValueConverter
    {
        public object True { get; set; }
        public object False { get; set; }
    
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            return Equals( value, parameter )? True:False;
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}