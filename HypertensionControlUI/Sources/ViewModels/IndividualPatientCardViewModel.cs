using System.Windows.Input;
using HypertensionControlUI.CompositionRoot;
using HypertensionControlUI.Models;
using HypertensionControlUI.Utils;

namespace HypertensionControlUI.ViewModels
{
    public class IndividualPatientCardViewModel : PageViewModelBase
    {
        #region Fields
        private readonly IViewProvider _viewProvider;
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly DbContextFactory _dbContextFactory;
        private Patient _patient;
        private PatientVisitData _patientVisitData;

        #endregion


        #region Auto-properties

        public ICommand ClassifyPatientCommand { get; private set; }

        #endregion


        #region Properties

        public PatientVisitData PatientVisitData
        {
            get { return _patientVisitData; }
            set
            {
                if ( Equals( value, _patientVisitData ) )
                    return;
                _patientVisitData = value;
                OnPropertyChanged();
            }
        }

        public Patient Patient
        {
            get { return _patient; }
            set
            {
                if ( Equals( value, _patient ) )
                    return;
                _patient = value;
                OnPropertyChanged();
            }
        }


        public string TreatmentDescription
        {
            get
            {
                if ( Patient.TreatmentDuration == null )
                    return "отсутствует";
                var result = Patient.TreatmentDuration.ToString();
                foreach ( var medcine in Patient.Medicine )
                    result += " " + medcine.Name + medcine.Dose + ";";
                return result;
            }
        }

        public string SmokingDescription
        {
            get
            {
                if ( PatientVisitData.Smoking.Type == SmokingType.Never )
                    return "никогда";
                var result = "";
                if ( PatientVisitData.Smoking.Type == SmokingType.InPast )
                    result = "в прошлом, "; 
                else
                    result = "да, ";
                result += "количество лет " + PatientVisitData.Smoking.DurationInYears;
                result += "количество сигарет в день " + PatientVisitData.Smoking.CigarettesPerDay;
                return result;
            }
        }

        #endregion


        #region Initialization

        public IndividualPatientCardViewModel( DbContextFactory dbContextFactory, IViewProvider viewProvider, MainWindowViewModel mainWindowViewModel )
        {
            _dbContextFactory = dbContextFactory;
            _viewProvider = viewProvider;
            _mainWindowViewModel = mainWindowViewModel;
            ClassifyPatientTunningCommand = new AsyncDelegateCommand( ClassifyPatientTunningCommandHandler );
            PatientsCommand = new AsyncDelegateCommand(o => _viewProvider.NavigateToPage<PatientsViewModel>(m =>
                                                                         {
                                                                             _mainWindowViewModel.Patient = null;
                                                                         }
                                                                     ));
        }

        private void ClassifyPatientTunningCommandHandler( object obj )
        {
            _viewProvider.NavigateToPage<ClassificationTunningViewModel>( model =>
            {
                model.Patient = Patient;
                model.PatientVisitData = PatientVisitData;
            });
        }
        

        public AsyncDelegateCommand ClassifyPatientTunningCommand { get; set; }
        public AsyncDelegateCommand PatientsCommand { get; set; }

        #endregion


        #region Non-public methods



        #endregion
    }
}
