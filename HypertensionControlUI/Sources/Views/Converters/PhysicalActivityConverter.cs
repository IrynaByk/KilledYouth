using System;
using System.Globalization;
using System.Windows.Data;
using HypertensionControlUI.Models;

namespace HypertensionControlUI.Views.Converters
{
    public class PhysicalActivityConverter : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            if (!(value is PhysicalActivity))
                return ""; 
            switch ( (PhysicalActivity)value )
            {
                case (PhysicalActivity.Never):
                    return "Никогда";
                case (PhysicalActivity.OncePerWeekOrLess):
                    return "Один раз в неделю или меньше";
                case (PhysicalActivity.FromOneToThreeTimesPerWeek):
                    return "Один три раза в неделю";
                case (PhysicalActivity.MoreThenTreeTimesPerWeek):
                    return "Больше трёх раз в неделю";
                default:
                    return "";
            }
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}