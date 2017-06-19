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
        #region Fields

        private readonly DbContextFactory _dbContextFactory;
        private readonly IViewProvider _viewProvider;
        private readonly MainWindowViewModel _mainWindowViewModel;
        private Patient _patient;
        private PatientVisitData _patientVisitData;
        private List<PossibleData> _possibleData;
        private double _possibleResult;
        private double _result;
        private ClassificationModel _selectedClassificationModel;

        #endregion


        #region Auto-properties

        public List<ClassificationModel> AvailableClassificationModels { get; set; }

        public PatientClassificatorFactory PatientClassificatorFactory { get; set; }

        //   public PatientClassificator PatientClassificator { get; set; }
        public ICommand ClassifyPatientCommand { get; }
        public ICommand PatientsCommand { get; }
        public ICommand ShowPatientCommand { get; set; }

        public ICommand ClassifyPossiblePatientCommand { get; }
        public Patient PossiblePatient { get; set; }
        public PatientVisitData PossibleLastVisitData { get; set; }

        #endregion


        #region Properties

        public List<PossibleData> PossibleData
        {
            get => _possibleData;
            set
            {
                if ( Equals( value, _possibleData ) )
                    return;
                _possibleData = value;
                OnPropertyChanged();
            }
        }

        public ClassificationModel SelectedClassificationModel
        {
            get => _selectedClassificationModel;
            set
            {
                if ( Equals( value, _selectedClassificationModel ) )
                    return;
                _selectedClassificationModel = value;

                PossiblePatient = Mapper.Map<Patient>( Patient );
                PossibleLastVisitData = PossiblePatient.PatientVisitDataHistory.OrderByDescending( d => d.VisitDate ).First();
                PossibleData = new PossibleChangeData().GeneratePossibleData( _selectedClassificationModel,
                                                                              PossiblePatient, PossibleLastVisitData );
                OnPropertyChanged();
            }
        }

        public double Result
        {
            get => _result;
            set
            {
                if ( value.Equals( _result ) )
                    return;
                _result = value;
                OnPropertyChanged();
            }
        }

        public double PossibleResult
        {
            get => _possibleResult;
            set
            {
                if ( value.Equals( _possibleResult ) )
                    return;
                _possibleResult = value;
                OnPropertyChanged();
            }
        }

        public Patient Patient
        {
            get => _patient;
            set
            {
                if ( Equals( value, _patient ) )
                    return;
                _patient = value;
                OnPropertyChanged();
            }
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

        #endregion


        #region Commands

        private void ClassifyPatientCommandHandler( object obj )
        {
            var patientClassificator = PatientClassificatorFactory.GetClassificator( SelectedClassificationModel );
            Result = patientClassificator.Classify( Patient, Patient.PatientVisitDataHistory.OrderByDescending( pvd => pvd.VisitDate ).First() );
        }

        private void ClassifyPossiblePatientCommandHandler( object obj )
        {
            var patientClassificator = PatientClassificatorFactory.GetClassificator( SelectedClassificationModel );
            PossibleResult = patientClassificator.Classify( PossiblePatient,
                                                            PossiblePatient.PatientVisitDataHistory.OrderByDescending( pvd => pvd.VisitDate ).First() );
        }

        #endregion


        #region Initialization

        public ClassificationTunningViewModel( DbContextFactory dbContextFactory,
                                               PatientClassificatorFactory patientClassificatorFactory,
                                               MainWindowViewModel mainWindowViewModel,
                                               IViewProvider viewProvider )
        {
            PatientClassificatorFactory = patientClassificatorFactory;
            _mainWindowViewModel = mainWindowViewModel;
            _viewProvider = viewProvider;
            _dbContextFactory = dbContextFactory;
            PossibleData = new List<PossibleData>();
            ClassifyPatientCommand = new AsyncDelegateCommand( ClassifyPatientCommandHandler );
            ClassifyPossiblePatientCommand = new AsyncDelegateCommand( ClassifyPossiblePatientCommandHandler );
            PatientsCommand = new AsyncDelegateCommand(o => _viewProvider.NavigateToPage<PatientsViewModel>(m => _mainWindowViewModel.Patient = null ));
            ShowPatientCommand = new AsyncDelegateCommand(o => _viewProvider.NavigateToPage<IndividualPatientCardViewModel>(m =>
            {
                m.Patient = Patient;
                m.PatientVisitData =
                    Patient
                        .PatientVisitDataHistory
                        .OrderByDescending(pvd => pvd.VisitDate)
                        .First();
            }));
        }

        #endregion


        #region Non-public methods

        private bool IsApplicable( Patient patient, PatientVisitData visitData, ClassificationModel classificationModel )
        {
            return classificationModel
                .Properties
                .All( p => PatientPropertyProvider.GetPropertyValue( p.Name, patient, visitData ) != null );
        }

        #endregion
    }

    public class PossibleData : ViewModelBase
    {
        #region Fields

        private readonly Patient _patient;
        private readonly PatientVisitData _patientVisitData;

        #endregion


        #region Auto-properties

        public string Key { get; set; }
        public object Value1 { get; set; }

        #endregion


        #region Properties

        public object Value2
        {
            get => PatientPropertyProvider.GetPropertyValue( Key, _patient, _patientVisitData );
            set => PatientPropertyProvider.UpdatePatientByProperty( Key, _patient, _patientVisitData, value );
        }

        #endregion


        #region Initialization

        public PossibleData( string key, Patient patient, PatientVisitData patientVisitData, object value2 = null )
        {
            Key = key;
            _patient = patient;
            _patientVisitData = patientVisitData;
            Value1 = PatientPropertyProvider.GetPropertyValue( key, patient, patientVisitData );
        }

        #endregion
    }

    public class PossibleChangeData
    {
        #region Fields

        private static Dictionary<string, string> _modelSourceFactors =
            new Dictionary<string, string>
            {
                ["Age"] = "Age",
                ["{PatientVisitData}.ObesityWaistCircumference"] = "{PatientVisitData}.WaistCircumference",
                ["{PatientVisitData}.WaistCircumference"] = "{PatientVisitData}.WaistCircumference",
                ["{PatientVisitData}.ObesityBMI"] = "{PatientVisitData}.Weight",
                ["{PatientVisitData}.BMI"] = "{PatientVisitData}.Weight",
                ["{PatientVisitData}.PhysicalActivity"] = "{PatientVisitData}.PhysicalActivity"
            };

        #endregion


        #region Public methods

        public List<PossibleData> GeneratePossibleData( ClassificationModel classificationModel, Patient patient, PatientVisitData possibleVisitData )
        {
            var possibleDataList = new List<PossibleData>();

            foreach ( var classificationModelProperty in classificationModel.Properties )
            {
                if ( _modelSourceFactors.TryGetValue( classificationModelProperty.Name, out var key ) )
                {
                    var possibleData = new PossibleData( key, patient, possibleVisitData );
                    possibleDataList.Add(possibleData);
                }
            }
            return possibleDataList;
        }

        #endregion


        #region Non-public methods

       
        #endregion
    }
}
