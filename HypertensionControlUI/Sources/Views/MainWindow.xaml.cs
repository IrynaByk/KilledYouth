using System;
using HypertensionControlUI.Properties;
using HypertensionControlUI.ViewModels;

namespace HypertensionControlUI.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowViewBase<MainWindowViewModel>
    {
        #region Initialization

        public MainWindow( MainWindowViewModel viewModel ) : base( viewModel )
        {
            if ( Settings.Default.UpgradeRequired )
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
            }

            InitializeComponent();

            Closed += MainWindow_Closed;
        }

        #endregion


        #region Event handlers

        private void MainWindow_Closed( object sender, EventArgs e )
        {
            Settings.Default.Save();
        }

        #endregion
    }
}
