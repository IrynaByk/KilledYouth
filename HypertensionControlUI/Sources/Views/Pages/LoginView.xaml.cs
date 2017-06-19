using System.Windows;
using HypertensionControlUI.ViewModels;

namespace HypertensionControlUI.Views.Pages
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginView : PageViewBase<LoginViewModel>
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void DEBUG_LoginAdmin_Click( object sender, RoutedEventArgs e )
        {
            ViewModel.Login = "admin";
            ViewModel.Password = "admin";
            ViewModel.LoginCommand?.Execute( null );
        }
    }
}
