using System;
using System.Globalization;
using System.Windows.Data;
using HypertensionControlUI.Models;

namespace HypertensionControlUI.Views.Converters
{
    public class FactorsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string))
                return "";
            switch ((string)value)
            {
                case "Age":
                    return "Возраст";
                case "{PatientVisitData}.WaistCircumference":
                    return "Объем талии";
                case "{PatientVisitData}.Weight":
                    return "Вес";
                case "{PatientVisitData}.PhysicalActivity":
                    return "Физ активность";
                default:
                    return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}