using System;
using System.Globalization;
using System.Windows.Data;
using HypertensionControlUI.Models;

namespace HypertensionControlUI.Views.Converters
{
    public class AlcoholConsumptionConverter : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            if (!(value is AlcoholСonsumption))
                return "";
            switch ((AlcoholСonsumption)value)
            {
                case (AlcoholСonsumption.Never):
                    return "Никогда или редко";
                case (AlcoholСonsumption.Monthly):
                    return "1-3 дозы в месяц";
                case (AlcoholСonsumption.Weekly):
                    return "1-6 доз в неделю";
                case (AlcoholСonsumption.Daily):
                    return "Больше 1 дозы в день";
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