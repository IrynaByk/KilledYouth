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
        public static readonly DependencyProperty CutOffValueProperty = DependencyProperty.Register(
            "CutOffValue", typeof (double), typeof (IllnessProbabilityChart), new PropertyMetadata( 0.5 ) );

        public double CutOffValue
        {
            get { return (double) GetValue( CutOffValueProperty ); }
            set { SetValue( CutOffValueProperty, value ); }
        }

        public static readonly DependencyProperty ThumbValueProperty = DependencyProperty.Register(
            "ThumbValue", typeof (double), typeof (IllnessProbabilityChart), new PropertyMetadata( default(double) ) );

        public double ThumbValue
        {
            get { return (double) GetValue( ThumbValueProperty ); }
            set { SetValue( ThumbValueProperty, value ); }
        }

        public IllnessProbabilityChart()
        {
            InitializeComponent();
        }
    }
}
