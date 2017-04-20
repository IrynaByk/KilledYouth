using System.Windows;
using System.Windows.Navigation;
using HypertensionControlUI.ViewModels;

namespace HypertensionControlUI.Views.Pages
{
    /// <summary>
    ///     Interaction logic for AddPatient.xaml
    /// </summary>
    public partial class AddPatientView : PageViewBase<AddPatientViewModel>
    {
        #region Properties

        public override AddPatientViewModel ViewModel
        {
            get { return DataContext as AddPatientViewModel; }
            set
            {
                DataContext = value;

                var contentElement = QuestionnaireFrame.Content as FrameworkElement;
                if ( contentElement != null )
                    contentElement.DataContext = ViewModel;
            }
        }

        #endregion


        #region Initialization

        public AddPatientView()
        {
            InitializeComponent();
        }

        #endregion


        #region Event handlers

        private void QuestionnaireFrame_Navigated( object sender, NavigationEventArgs e )
        {
            var contentElement = e.Content as FrameworkElement;
            if ( contentElement != null )
                contentElement.DataContext = ViewModel;
        }

        #endregion
    }
}
