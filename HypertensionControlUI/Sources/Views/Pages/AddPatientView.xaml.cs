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

        /// <summary>
        ///     View-model represented by the current view.
        /// </summary>
        public override AddPatientViewModel ViewModel
        {
            get => DataContext as AddPatientViewModel;
            set
            {
                DataContext = value;
                UpdateQuestionnaireFrameViewModel();
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
            if ( ReferenceEquals( sender, QuestionnaireFrame ) )
                UpdateQuestionnaireFrameViewModel();
        }

        /// <summary>
        ///     Sets the View-Model of the QuestionnaireFrame to the actual value.
        /// </summary>
        private void UpdateQuestionnaireFrameViewModel()
        {
            if ( QuestionnaireFrame.Content is FrameworkElement contentElement )
                contentElement.DataContext = ViewModel;
        }

        #endregion
    }
}
