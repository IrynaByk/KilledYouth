using System;
using System.Globalization;
using System.Windows.Data;

namespace HypertensionControlUI.Views.Converters
{
    public class BirthDateToYears : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            try
            {
                return (DateTime.Today - System.Convert.ToDateTime( value )).Days/365;
            }
            catch
            {
                return Binding.DoNothing;
            }
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}