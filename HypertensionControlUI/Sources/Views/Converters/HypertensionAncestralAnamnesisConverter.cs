using System;
using System.Globalization;
using System.Windows.Data;
using HypertensionControlUI.Models;

namespace HypertensionControlUI.Views.Converters
{
    public class HypertensionAncestralAnamnesisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is HypertensionAncestralAnamnesis))
                return "";
            switch ((HypertensionAncestralAnamnesis)value)
            {
                case (HypertensionAncestralAnamnesis.None):
                    return "Отсутствует";
                case (HypertensionAncestralAnamnesis.Mother):
                    return "Мать";
                case (HypertensionAncestralAnamnesis.Father):
                    return "Отец";
                case (HypertensionAncestralAnamnesis.BothMotherAndFather):
                    return "Мать и отец";
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