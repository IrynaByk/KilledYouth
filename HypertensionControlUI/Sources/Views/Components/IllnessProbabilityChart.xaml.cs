using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HypertensionControlUI.Sources.Views.Components
{
    /// <summary>
    /// Interaction logic for IllnessProbabilityChart.xaml
    /// </summary>
    public partial class IllnessProbabilityChart : UserControl
    {
        public static readonly DependencyProperty CutOffValueProperty1 = DependencyProperty.Register(
            "CutOffValue1", typeof (double), typeof (IllnessProbabilityChart), new PropertyMetadata( 0.5 ) );

        public double CutOffValue1
        {
            get => (double) GetValue( CutOffValueProperty1 );
            set => SetValue( CutOffValueProperty1, value );
        }
        public static readonly DependencyProperty CutOffValueProperty2 = DependencyProperty.Register(
            "CutOffValue2", typeof(double), typeof(IllnessProbabilityChart), new PropertyMetadata(0.5));

        public double CutOffValue2
        {
            get => (double)GetValue(CutOffValueProperty2);
            set => SetValue(CutOffValueProperty2, value);
        }

        public double MiddleTrackValue
        {
            get => (double) (CutOffValue1 + (CutOffValue2 - CutOffValue1) / 2);
        }

        public static readonly DependencyProperty ThumbValueProperty = DependencyProperty.Register(
            "ThumbValue", typeof (double), typeof (IllnessProbabilityChart), new PropertyMetadata( default(double) ) );

        public double ThumbValue
        {
            get => (double) GetValue( ThumbValueProperty );
            set => SetValue( ThumbValueProperty, value );
        }

        public IllnessProbabilityChart()
        {
            InitializeComponent();
        }
    }
}
