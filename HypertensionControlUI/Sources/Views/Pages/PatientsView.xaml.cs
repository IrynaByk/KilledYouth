using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using HypertensionControlUI.ViewModels;

namespace HypertensionControlUI.Views.Pages
{
    /// <summary>
    ///     Interaction logic for Window1.xaml
    /// </summary>
    public partial class PatientsView : PageViewBase<PatientsViewModel>
    {
        #region Initialization

        public PatientsView()
        {
            InitializeComponent();
        }

        #endregion


        private void PatientsView_OnLoaded( object sender, RoutedEventArgs e )
        {
            var selectedPatientIndex = PatientsList.SelectedIndex;
            if ( selectedPatientIndex != -1 )
            {
                VirtualizingStackPanel vsp = (VirtualizingStackPanel)typeof(ItemsControl).InvokeMember(
                    "_itemsHost",
                    BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic, null,
                    PatientsList, null
                );
                double scrollHeight = vsp.ScrollOwner.ScrollableHeight;
                double offset = scrollHeight * selectedPatientIndex / PatientsList.Items.Count;
                vsp.SetVerticalOffset(offset);
            }
        }
    }
}
