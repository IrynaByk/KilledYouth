using System;
using System.Globalization;
using System.Windows.Data;
using HypertensionControl.Domain.Models.Values;

namespace HypertensionControlUI.Views.Converters
{
    public class HStageConverter : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            if ( value is null )
                return "нет данных";

            if ( !(value is HypertensionStage stageValue) )
                return "";

            switch ( stageValue )
            {
                case (HypertensionStage.Healthy):
                    return "здоров";
                case HypertensionStage.Stage1:
                    return "1";
                case HypertensionStage.Stage2:
                    return "2";
                case HypertensionStage.Stage3:
                    return "3";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}
