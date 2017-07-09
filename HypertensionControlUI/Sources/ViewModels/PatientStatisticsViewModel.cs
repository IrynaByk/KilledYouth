using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using HypertensionControl.Domain.Models;
using HypertensionControl.Domain.Models.Values;
using HypertensionControl.Domain.Services;
using HypertensionControlUI.CompositionRoot;
using HypertensionControlUI.Interfaces;
using HypertensionControlUI.Utils;

namespace HypertensionControlUI.ViewModels
{
    public class PatientStatisticsViewModel : PageViewModelBase
    {
        #region Fields

        private readonly MainWindowViewModel _mainWindowViewModel;

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IViewProvider _viewProvider;
        private List<Patient> _patients;
        private List<PatientStatisticsRowViewModel> _patientStatisticsData;

        #endregion


        #region Auto-properties

        public ICommand PatientsCommand { get; }

        public List<ClassificationModel> ClassificationModels { get; }

        public PatientClassificatorFactory PatientClassificatorFactory { get; }

        #endregion


        #region Properties

        public List<Patient> Patients
        {
            get => _patients;
            set => Set( ref _patients, value );
        }

        public List<PatientStatisticsRowViewModel> PatientStatisticsData
        {
            get => _patientStatisticsData;
            set => Set( ref _patientStatisticsData, value );
        }

        #endregion


        #region Commands

        private void PatientsCommandHandler( object o )
        {
            _viewProvider.NavigateToPage<PatientsViewModel>( m => _mainWindowViewModel.Patient = null );
        }

        #endregion


        #region Initialization

        public PatientStatisticsViewModel( IUnitOfWorkFactory unitOfWorkFactory,
                                           PatientClassificatorFactory patientClassificatorFactory,
                                           IViewProvider viewProvider,
                                           MainWindowViewModel mainWindowViewModel )
        {
            PatientClassificatorFactory = patientClassificatorFactory;
            _unitOfWorkFactory = unitOfWorkFactory;
            _viewProvider = viewProvider;
            _mainWindowViewModel = mainWindowViewModel;

            using ( var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork() )
            {
                Patients = unitOfWork.PatientsRepository.GetAllPatients().ToList();
                ClassificationModels = unitOfWork.ClassificationModelsRepository.GetAllClassificationModels().ToList();
            }

            PatientStatisticsData = new List<PatientStatisticsRowViewModel>();
            foreach ( var patient in Patients )
            {
                var patientSatisticsRowData = new PatientStatisticsRowViewModel( patient, ClassificationModels[0], ClassificationModels[1] );
                PatientStatisticsData.Add( patientSatisticsRowData );
            }

            PatientsCommand = new AsyncDelegateCommand( PatientsCommandHandler );
        }

        #endregion
    }

    public class PatientStatisticsRowViewModel : ViewModelBase
    {
        #region Fields

        private readonly ClassificationModel _modelWithGene;
        private readonly ClassificationModel _modelWithoutGene;
        private readonly Patient _patient;
        private double _cutOff;

        #endregion


        #region Properties

        public double CutOff
        {
            get => _cutOff;
            set
            {
                if ( value == _cutOff )
                {
                    return;
                }
                _cutOff = value;
                OnPropertyChanged( nameof(ModelWithGeneLastResult) );
            }
        }

        public double? ModelWithGeneLastResult => GetModelScore( _modelWithGene );
        public double? ModelWithoutGeneLastResult => GetModelScore( _modelWithoutGene );

        public string Name => _patient.Surname + " " + _patient.Name + " " + _patient.MiddleName;
        public HypertensionStage? LastStage => _patient.HypertensionStage;

        #endregion


        #region Initialization

        public PatientStatisticsRowViewModel( Patient patient, ClassificationModel classificationModel1, ClassificationModel classificationModel2 )
        {
            _patient = patient;
            _modelWithGene = classificationModel1;
            _modelWithoutGene = classificationModel2;
        }

        #endregion


        #region Public methods

        public string ModelPrediction( PatientVisit patientVisit, ClassificationModel model )
        {
            if ( !IsApplicable( _patient, patientVisit, model ) )
            {
                return "нет данных";
            }

            var score = Score( patientVisit, model );
            var description = (score > CutOff ? "болен" : "здоров");

            return $"{score:F4}/{description}";
        }

        #endregion


        #region Non-public methods

        private double? GetModelScore( ClassificationModel model )
        {
            var patientVisit = _patient.LastVisit;
            if ( !IsApplicable( _patient, patientVisit, model ) )
            {
                return null;
            }
            return Score( patientVisit, model );
        }

        private double Score( PatientVisit patientVisit, ClassificationModel model )
        {
            var classificator = new PatientClassificator( model );
            var score = classificator.Classify( new { Patient = _patient, PatientVisit = patientVisit } );
            return score;
        }

        private bool IsApplicable( Patient patient, PatientVisit visitData, ClassificationModel classificationModel )
        {
            return classificationModel
                .Properties
                .All( p => PatientPropertyProvider.GetPropertyValue( new { Patient = patient, PatientVisit = visitData }, p.Name ) != null );
        }

        #endregion
    }
}