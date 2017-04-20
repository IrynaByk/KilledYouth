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
using HypertensionControlUI.Properties;
using HypertensionControlUI.ViewModels;

namespace HypertensionControlUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowViewBase<MainWindowViewModel>
    {
        public MainWindow(MainWindowViewModel viewModel ):base(viewModel)
        {
            if ( Settings.Default.UpgradeRequired )
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
            }

            InitializeComponent();

            Closed += MainWindow_Closed;
        }
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Settings.Default.Save();
        }
    }
}
