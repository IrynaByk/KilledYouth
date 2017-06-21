using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows.Input;
using AutoMapper;
using HypertensionControlUI.CompositionRoot;
using HypertensionControlUI.Models;
using HypertensionControlUI.Services;
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
            ["PatientVisitData.ObesityWaistCircumference"] = "PatientVisitData.WaistCircumference",
            ["PatientVisitData.WaistCircumference"] = "PatientVisitData.WaistCircumference",
            ["PatientVisitData.ObesityBMI"] = "PatientVisitData.Weight",
            ["PatientVisitData.BMI"] = "PatientVisitData.Weight",
            ["PatientVisitData.PhysicalActivity"] = "PatientVisitData.PhysicalActivity"
        };

        #endregion


        #region Fields

        private readonly DbContextFactory _dbContextFactory;
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly PatientClassificatorFactory _patientClassificatorFactory;
        private readonly IViewProvider _viewProvider;

        private List<CorrectablePatientPropertyViewModel> _correctableProperties;
        private double _correctedResult;
        private Patient _patient;
        private PatientVisitData _patientVisitData;
        private double _result;
        private ClassificationModel _selectedClassificationModel;

        #endregion


        #region Auto-properties

        public List<ClassificationModel> AvailableClassificationModels { get; set; }

        public ICommand ClassifyPossiblePatientCommand { get; }

        public EditablePatientVisitData CorrectedLastVisitData { get; set; }
        public EditablePatient CorrectedPatient { get; set; }

        //   public PatientClassificator PatientClassificator { get; set; }
        public ICommand PatientsCommand { get; }

        public ICommand ShowPatientCommand { get; set; }

        #endregion


        #region Properties

        public List<CorrectablePatientPropertyViewModel> CorrectableProperties
        {
            get => _correctableProperties;
            set => Set( ref _correctableProperties, value );
        }

        public double CorrectedResult
        {
            get => _correctedResult;
            set => Set( ref _correctedResult, value );
        }

        public Patient Patient
        {
            get => _patient;
            set => Set( ref _patient, value );
        }

        public PatientVisitData PatientVisitData
        {
            get => _patientVisitData;
            set
            {
                if ( Equals( value, _patientVisitData ) )
                    return;
                _patientVisitData = value;
                OnPropertyChanged();

                using ( var db = _dbContextFactory.GetDbContext() )
                {
                    AvailableClassificationModels = db.ClassificationModels
                                                      .Include( model => model.LimitPoints )
                                                      .Include( model => model.Properties.Select( property => property.Entries ) )
                                                      .ToList()
                                                      .Where( m => IsApplicable( Patient, PatientVisitData, m ) )
                                                      .ToList();
                }
            }
        }

        public double Result
        {
            get => _result;
            set => Set( ref _result, value );
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
                    _selectedClassificationModel = value;
                    ClassifyPatient();
                    ResetCorrectedPatient();
                }
            }
        }

        #endregion


        #region Commands

        private void ClassifyPossiblePatientCommandHandler( object obj )
        {
            var patientClassificator = _patientClassificatorFactory.GetClassificator( SelectedClassificationModel );
            CorrectedResult = patientClassificator.Classify( new
            {
                Patient = CorrectedPatient,
                PatientVisitData = CorrectedPatient.PatientVisitDataHistory.OrderByDescending( pvd => pvd.VisitDate ).First()
            } );
        }

        #endregion


        #region Initialization

        public ClassificationTunningViewModel( DbContextFactory dbContextFactory,
                                               PatientClassificatorFactory patientClassificatorFactory,
                                               MainWindowViewModel mainWindowViewModel,
                                               IViewProvider viewProvider )
        {
            //  Inject services
            _patientClassificatorFactory = patientClassificatorFactory;
            _mainWindowViewModel = mainWindowViewModel;
            _viewProvider = viewProvider;
            _dbContextFactory = dbContextFactory;

            CorrectableProperties = new List<CorrectablePatientPropertyViewModel>();

            //  Init commands
            ClassifyPossiblePatientCommand = new AsyncDelegateCommand( ClassifyPossiblePatientCommandHandler );
            PatientsCommand = new AsyncDelegateCommand( o => _viewProvider.NavigateToPage<PatientsViewModel>( m => _mainWindowViewModel.Patient = null ) );
            ShowPatientCommand = new AsyncDelegateCommand( o => _viewProvider.NavigateToPage<IndividualPatientCardViewModel>( m =>
            {
                m.Patient = Patient;
                m.PatientVisitData = Patient.LastVisitData;
            } ) );
        }

        #endregion


        #region Public methods

        /// <summary>
        ///     Maps the classification model properties to the initial patient properties they depend on.
        /// </summary>
        public List<CorrectablePatientPropertyViewModel> GeneratePatientCorrectionData( ClassificationModel classificationModel,
                                                                                        EditablePatient patient,
                                                                                        EditablePatientVisitData possibleVisitData )
        {
            var possibleDataList = new List<CorrectablePatientPropertyViewModel>();

            //  Select the model properties that can be corrected
            foreach ( var classificationModelProperty in classificationModel.Properties )
            {
                if ( ModelSourceFactors.TryGetValue( classificationModelProperty.Name, out var propertyKey ) )
                {
                    var possibleData = new CorrectablePatientPropertyViewModel( propertyKey, patient, possibleVisitData );
                    possibleDataList.Add( possibleData );
                }
            }
            return possibleDataList;
        }

        #endregion


        #region Non-public methods

        private void ResetCorrectedPatient()
        {
            CorrectedPatient = Mapper.Map<EditablePatient>( Patient );
            CorrectedLastVisitData = CorrectedPatient.PatientVisitDataHistory.OrderByDescending( d => d.VisitDate ).First();
            CorrectableProperties = GeneratePatientCorrectionData( _selectedClassificationModel, CorrectedPatient, CorrectedLastVisitData );
        }

        private void ClassifyPatient()
        {
            var patientClassificator = _patientClassificatorFactory.GetClassificator( SelectedClassificationModel );
            Result = patientClassificator.Classify( new { Patient, PatientVisitData = Patient.LastVisitData } );
        }

        private bool IsApplicable( Patient patient, PatientVisitData visitData, ClassificationModel classificationModel )
        {
            var dataSource = new { Patient = patient, PatientVisitData = visitData };

            return classificationModel.Properties.All( p => PatientPropertyProvider.GetPropertyValue( p.Name, dataSource ) != null );
        }

        #endregion
    }

    /// <summary>
    ///     Represents a single patient property which value can be corrected.
    /// </summary>
    public class CorrectablePatientPropertyViewModel : ViewModelBase
    {
        #region Fields

        private readonly object _dataSource;

        #endregion


        #region Auto-properties

        /// <summary>
        ///     The property key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     Original propery value.
        /// </summary>
        public object OriginalValue { get; }

        #endregion


        #region Properties

        /// <summary>
        ///     Edited property value.
        /// </summary>
        public object CorrectedValue
        {
            get => PatientPropertyProvider.GetPropertyValue( Key, _dataSource );
            set
            {
                PatientPropertyProvider.UpdatePatientByProperty( Key, _dataSource, value );
                CorrectedValueChanged?.Invoke();
            }
        }

        #endregion


        #region Events and invocation

        /// <summary>
        ///     Notifies about change in the property value.
        /// </summary>
        public event Action CorrectedValueChanged;

        #endregion


        #region Initialization

        /// <summary>
        ///     Creates the view-model for the correctable property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="patient">The patient reference.</param>
        /// <param name="patientVisitData">The patient visit data reference.</param>
        public CorrectablePatientPropertyViewModel( string key, EditablePatient patient, EditablePatientVisitData patientVisitData )
        {
            Key = key;
            _dataSource = new { Patient = patient, PatientVisitData = patientVisitData };
            OriginalValue = PatientPropertyProvider.GetPropertyValue( key, _dataSource );
        }

        #endregion
    }

    public class EditablePatient
    {
        #region Auto-properties

        public string AccompanyingIllnesses { get; set; }
        public string Address { get; set; }
        public int Age { get; set; }
        public int? AGT_AGTR2 { get; set; }
        public DateTime BirthDate { get; set; }
        public long BirthDateTicks { get; set; }
        public string BirthPlace { get; set; }
        public virtual Clinic Clinic { get; set; }
        public string CreatedBy { get; set; }
        public string Diagnosis { get; set; }
        public bool FemaleHeredity { get; set; }
        public GenderType Gender { get; set; }
        public virtual ICollection<Gene> Genes { get; set; }
        public HypertensionAncestralAnamnesis HypertensionAncestralAnamnesis { get; set; }
        public double HypertensionDuration { get; set; }
        public int Id { get; set; }
        public bool MaleHeredity { get; set; }
        public virtual ICollection<Medicine> Medicine { get; set; }
        public string MiddleName { get; set; }
        public string Name { get; set; }
        public string Nationality { get; set; }
        public virtual ICollection<EditablePatientVisitData> PatientVisitDataHistory { get; set; }
        public EditablePatientVisitData LastVisitData => PatientVisitDataHistory.OrderByDescending(pvd => pvd.VisitDate).First();
        public string Phone { get; set; }
        public HypertensionStage? Stage { get; set; }
        public string Surname { get; set; }
        public double? TreatmentDuration { get; set; }

        #endregion
    }

    public class EditablePatientVisitData
    {
        #region Auto-properties

        public AlcoholСonsumption AlcoholСonsumption { get; set; }
        public BloodPressure BloodPressure { get; set; }
        public double? BMI { get; set; }
        public int? DepressionPointsCES_D { get; set; }
        public DietaryHabits DietaryHabits { get; set; }
        public double Height { get; set; }
        public HypertensionStage? HypertensionStage { get; set; }
        public int Id { get; set; }
        public bool? ObesityBMI { get; set; }
        public bool? ObesityWaistCircumference { get; set; }
        public virtual Patient Patient { get; set; }
        public PhysicalActivity? PhysicalActivity { get; set; }
        public SaltSensitivityTest SaltSensitivity { get; set; }
        public Smoking Smoking { get; set; }
        public int? StressPointsPSM_25 { get; set; }
        public double TemporaryBMI { get; set; }
        public DateTime VisitDate { get; set; }
        public long VisitDateTicks { get; set; }
        public double WaistCircumference { get; set; }
        public double Weight { get; set; }

        #endregion
    }
}
