using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace HypertensionControlUI.Sources.Views.Components
{
    /// <summary>
    ///     Interaction logic for IllnessBarChart.xaml
    /// </summary>
    public partial class IllnessBarChart : UserControl
    {
        #region Constants

        public static readonly DependencyProperty ProbabilityBeforeProperty = DependencyProperty.Register(
            "ProbabilityBefore", typeof(double), typeof(IllnessBarChart), new PropertyMetadata( 0.5 ) );

        public static readonly DependencyProperty ProbabilityAfterProperty = DependencyProperty.Register(
            "ProbabilityAfter", typeof(double), typeof(IllnessBarChart), new PropertyMetadata( 0.5 ) );

        public static readonly DependencyProperty CutOffValue0Property = DependencyProperty.Register(
            "CutOffValue0", typeof(double), typeof(IllnessBarChart), new PropertyMetadata( 0.36 ) );

        public static readonly DependencyProperty CutOffValue1Property = DependencyProperty.Register(
            "CutOffValue1", typeof(double), typeof(IllnessBarChart), new PropertyMetadata( 0.73 ) );

        #endregion


        #region Auto-properties

        public static InvertingConverter InvertingConverterInstance { get; } = new InvertingConverter();
        public static BarHeightConverter BarHeightConverterInstance { get; } = new BarHeightConverter();
        public static LineVerticalPositionConverter LineVerticalPositionConverterInstance { get; } = new LineVerticalPositionConverter();

        #endregion


        #region Properties

        public double CutOffValue0
        {
            get => (double) GetValue( CutOffValue0Property );
            set => SetValue( CutOffValue0Property, value );
        }

        public double CutOffValue1
        {
            get => (double) GetValue( CutOffValue1Property );
            set => SetValue( CutOffValue1Property, value );
        }

        public double ProbabilityBefore
        {
            get => (double) GetValue( ProbabilityBeforeProperty );
            set => SetValue( ProbabilityBeforeProperty, value );
        }

        public double ProbabilityAfter
        {
            get => (double) GetValue( ProbabilityAfterProperty );
            set => SetValue( ProbabilityAfterProperty, value );
        }

        #endregion


        #region Initialization

        public IllnessBarChart()
        {
            InitializeComponent();
        }

        #endregion


        #region Nested type: BarHeightConverter

        public class BarHeightConverter : IMultiValueConverter
        {
            #region Public methods

            public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
            {
                if ( values.Length >= 2 && values[0] is double value && values[1] is double fullHeight )
                    return fullHeight * value;

                return Binding.DoNothing;
            }

            public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion


        #region Nested type: InvertingConverter

        public class InvertingConverter : IValueConverter
        {
            #region Public methods

            public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
            {
                if ( value is double d )
                    return 1 / d;
                return Binding.DoNothing;
            }

            public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion


        #region Nested type: LineVerticalPositionConverter

        public class LineVerticalPositionConverter : IMultiValueConverter
        {
            #region Public methods

            public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
            {
                if ( values.Length >= 2 && values[0] is double value && values[1] is double fullHeight )
                    return Math.Round( fullHeight * (1 - value) );

                return Binding.DoNothing;
            }

            public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion
    }
}
