using System;
using System.Globalization;
using System.Windows.Data;

namespace HypertensionControlUI.Views.Converters
{
    public class DateToStringConverter : IValueConverter
    {
        public static DateToStringConverter Instance { get; } = new DateToStringConverter();
        
        #region Public methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ( value is DateTime date )
            {
                return date.ToString("dd.MM.yyyy");
            }
            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
