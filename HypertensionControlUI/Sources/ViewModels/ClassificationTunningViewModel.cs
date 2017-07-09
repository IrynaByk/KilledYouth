using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using AutoMapper;
using HypertensionControl.Domain.Models;
using HypertensionControl.Domain.Services;
using HypertensionControl.Persistence.Entities;
using HypertensionControlUI.CompositionRoot;
using HypertensionControlUI.Interfaces;
using HypertensionControlUI.Utils;

namespace HypertensionControlUI.ViewModels
{
    public class ClassificationTunningViewModel : PageViewModelBase
    {
        #region Constants

        /// <summary>
        ///     Possible mappings of a classification model properties to the initial patient properties they depend on.
        /// </summary>
        private static readonly Dictionary<string, string> ModelSourceFactors = new Dictionary<string, string>
        {
            ["Patient.Age"] = "Patient.Age",
            ["PatientVisit.ObesityWaistCircumference"] = "PatientVisit.WaistCircumference",
            ["PatientVisit.WaistCircumference"] = "PatientVisit.WaistCircumference",
            ["PatientVisit.ObesityBMI"] = "PatientVisit.Weight",
            ["PatientVisit.BMI"] = "PatientVisit.Weight",
            ["PatientVisit.PhysicalActivity"] = "PatientVisit.PhysicalActivity"
        };

        #endregion


        #region Fields

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IViewProvider _viewProvider;
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly PatientClassificatorFactory _patientClassificatorFactory;
        private double _classificationResult;

        private List<EditablePropertyViewModel> _correctableProperties;
        private double _correctedClassificationResult;
        private Patient _patient;
        private PatientVisit _patientVisit;
        private ClassificationModel _selectedClassificationModel;

        #endregion


        #region Auto-properties

        public List<ClassificationModel> AvailableClassificationModels { get; set; }

        public PatientVisit CorrectedLastVisitData { get; set; }
        public Patient CorrectedPatient { get; set; }

        public ICommand PatientsCommand { get; }
        public ICommand ShowPatientCommand { get; set; }

        #endregion


        #region Properties

        /// <summary>
        ///     The result of classification applied to the patient
        /// </summary>
        public double ClassificationResult
        {
            get => _classificationResult;
            set => Set( ref _classificationResult, value );
        }

        /// <summary>
        ///     List of the correctable patient properties.
        /// </summary>
        public List<EditablePropertyViewModel> CorrectableProperties
        {
            get => _correctableProperties;
            set => Set( ref _correctableProperties, value );
        }

        /// <summary>
        ///     The result of classification applied to the corrected patient.
        /// </summary>
        public double CorrectedClassificationResult
        {
            get => _correctedClassificationResult;
            set => Set( ref _correctedClassificationResult, value );
        }

        /// <summary>
        ///     The patient to which the classification model is applied.
        /// </summary>
        public Patient Patient
        {
            get => _patient;
            set => Set( ref _patient, value );
        }

        public PatientVisit PatientVisit
        {
            get => _patientVisit;
            set
            {
                if ( Set( ref _patientVisit, value ) )
                    RefreshAvailableClassificationModels();
            }
        }

        /// <summary>
        ///     The selected classification model. Upon change the prediction is calculated.
        /// </summary>
        public ClassificationModel SelectedClassificationModel
        {
            get => _selectedClassificationModel;
            set
            {
                if ( Set( ref _selectedClassificationModel, value ) )
                {
                    ClassifyPatient();
                    ResetCorrectedPatient();
                    ClassifyCorrectedPatient();
                }
            }
        }

        #endregion


        #region Initialization

        public ClassificationTunningViewModel( PatientClassificatorFactory patientClassificatorFactory,
                                               MainWindowViewModel mainWindowViewModel,
                                               IViewProvider viewProvider,
                                               IUnitOfWorkFactory unitOfWorkFactory )
        {
            //  Inject services
            _patientClassificatorFactory = patientClassificatorFactory;
            _mainWindowViewModel = mainWindowViewModel;
            _viewProvider = viewProvider;
            _unitOfWorkFactory = unitOfWorkFactory;

            CorrectableProperties = new List<EditablePropertyViewModel>();

            //  Init commands

            PatientsCommand = new AsyncDelegateCommand( o => _viewProvider.NavigateToPage<PatientsViewModel>( m => _mainWindowViewModel.Patient = null ) );

            ShowPatientCommand = new AsyncDelegateCommand( o => _viewProvider.NavigateToPage<IndividualPatientCardViewModel>( m =>
            {
                m.Patient = Patient;
                m.PatientVisit = Patient.LastVisit;
            } ) );
        }

        #endregion


        #region Public methods

        /// <summary>
        ///     Maps the classification model properties to the initial patient properties they depend on.
        /// </summary>
        public List<EditablePropertyViewModel> GeneratePatientCorrectionData( ClassificationModel classificationModel,
                                                                              Patient patient,
                                                                              PatientVisit possibleVisitData )
        {
            var correctedDataList = new List<EditablePropertyViewModel>();

            //  Select the model properties that can be corrected
            foreach ( var classificationModelProperty in classificationModel.Properties )
            {
                if ( ModelSourceFactors.TryGetValue( classificationModelProperty.Name, out var propertyKey ) )
                {
                    var correctableProperty = new EditablePropertyViewModel( propertyKey, patient, possibleVisitData );
                    correctableProperty.CorrectedValueChanged += PossibleData_CorrectedValueChanged;
                    correctedDataList.Add( correctableProperty );
                }
            }
            return correctedDataList;
        }

        #endregion


        #region Event handlers

        private void PossibleData_CorrectedValueChanged()
        {
            ClassifyCorrectedPatient();
        }

        #endregion


        #region Non-public methods


        private void RefreshAvailableClassificationModels()
        {
            using ( var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork() )
            {
                AvailableClassificationModels = unitOfWork.ClassificationModelsRepository.GetAllClassificationModels()
                                                          .Where( m => IsApplicable( Patient, PatientVisit, m ) )
                                                          .ToList();

                SelectedClassificationModel = AvailableClassificationModels.FirstOrDefault();
            }
        }

        private void ResetCorrectedPatient()
        {
            CorrectedPatient = Mapper.Map<Patient>( Patient );
            CorrectedLastVisitData = CorrectedPatient.LastVisit;
            CorrectableProperties = GeneratePatientCorrectionData( _selectedClassificationModel, CorrectedPatient, CorrectedLastVisitData );
        }

        private void ClassifyCorrectedPatient()
        {
            var patientClassificator = _patientClassificatorFactory.GetClassificator( SelectedClassificationModel );
            CorrectedClassificationResult = patientClassificator.Classify( new
            {
                Patient = CorrectedPatient,
                PatientVisit = CorrectedPatient.LastVisit
            } );
        }

        private void ClassifyPatient()
        {
            var patientClassificator = _patientClassificatorFactory.GetClassificator( SelectedClassificationModel );
            ClassificationResult = patientClassificator.Classify( new { Patient, PatientVisit = Patient.LastVisit } );
        }

        private bool IsApplicable( Patient patient, PatientVisit visitData, ClassificationModel classificationModel )
        {
            var dataSource = new { Patient = patient, PatientVisit = visitData };

            return classificationModel.Properties.All( p => PatientPropertyProvider.GetPropertyValue( dataSource, p.Name ) != null );
        }

        #endregion
    }
}
