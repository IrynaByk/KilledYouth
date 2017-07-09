using System.Windows.Input;
using HypertensionControl.Domain.Models;
using HypertensionControl.Domain.Models.Values;
using HypertensionControlUI.Interfaces;
using HypertensionControlUI.Utils;

namespace HypertensionControlUI.ViewModels
{
    public class IndividualPatientCardViewModel : PageViewModelBase
    {
        #region Fields

        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IViewProvider _viewProvider;
        private Patient _patient;
        private PatientVisit _patientVisit;

        #endregion


        #region Auto-properties

        public ICommand ClassifyPatientCommand { get; private set; }

        public AsyncDelegateCommand ClassifyPatientTunningCommand { get; set; }
        public AsyncDelegateCommand PatientsCommand { get; set; }

        #endregion


        #region Properties

        public PatientVisit PatientVisit
        {
            get => _patientVisit;
            set => Set( ref _patientVisit, value );
        }

        public Patient Patient
        {
            get => _patient;
            set => Set( ref _patient, value );
        }

        public string TreatmentDescription
        {
            get
            {
                if ( Patient.TreatmentDuration == null )
                {
                    return "отсутствует";
                }

                return Patient.TreatmentDuration + string.Join( "; ", Patient.Medicine );
            }
        }

        public string SmokingDescription
        {
            get
            {
                var smoking = PatientVisit.Smoking;

                return smoking.Type == SmokingType.Never
                    ? "никогда"
                    : $"{(smoking.Type == SmokingType.InPast ? "в прошлом" : "да")}, количество лет {smoking.DurationInYears}, количество сигарет в день {smoking.CigarettesPerDay}";
            }
        }

        #endregion


        #region Commands

        private void ClassifyPatientTunningCommandHandler( object obj )
        {
            _viewProvider.NavigateToPage<ClassificationTunningViewModel>( model =>
            {
                model.Patient = Patient;
                model.PatientVisit = PatientVisit;
            } );
        }

        #endregion


        #region Initialization

        public IndividualPatientCardViewModel( IViewProvider viewProvider, MainWindowViewModel mainWindowViewModel )
        {
            _viewProvider = viewProvider;
            _mainWindowViewModel = mainWindowViewModel;
            ClassifyPatientTunningCommand = new AsyncDelegateCommand( ClassifyPatientTunningCommandHandler );
            PatientsCommand = new AsyncDelegateCommand( o => _viewProvider.NavigateToPage<PatientsViewModel>( m => _mainWindowViewModel.Patient = null ) );
        }

        #endregion
    }
}
