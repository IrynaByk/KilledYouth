using System;
using System.Windows;
using System.Windows.Data;

namespace HypertensionControlUI.Views.Converters
{
    public class EnumBooleanConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ( parameter == null )
                return DependencyProperty.UnsetValue;

            if (!Enum.IsDefined(value.GetType(), value))
                return DependencyProperty.UnsetValue;

            if ( !Enum.IsDefined( value.GetType(), parameter ) )
                return DependencyProperty.UnsetValue;

            return value.Equals( parameter );
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ( !(value is bool) || !(bool) value )
                return Binding.DoNothing;
            return parameter;
        }
        #endregion
    }
}