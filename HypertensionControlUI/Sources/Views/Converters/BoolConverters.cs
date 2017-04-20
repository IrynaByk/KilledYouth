using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HypertensionControlUI.Views.Converters
{
    public static class BoolConverters
    {
        #region Constants

        public static readonly NotConverter Not = new NotConverter();
        public static readonly EnumBooleanConverter EnumConverter = new EnumBooleanConverter();

        public static readonly BooleanToVisibilityConverter BoolToVisibility = new BooleanToVisibilityConverter();
        public static readonly NotBooleanToVisibilityConverter NotBoolToVisibility = new NotBooleanToVisibilityConverter();

        #endregion
    }

    public class NotConverter : IValueConverter
    {
        #region Public methods

        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            return !(value is bool && (bool) value);
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            return !(value is bool && (bool)value);
        }

        #endregion
    }
    public class BooleanToVisibilityConverter : IValueConverter
    {
        #region Public methods

        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            return (value is bool && (bool) value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class NotBooleanToVisibilityConverter : IValueConverter
    {
        #region Public methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
