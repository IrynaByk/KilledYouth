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
        #region Fields

        private readonly DbContextFactory _dbContextFactory;
        private Patient _patient;
        private PatientVisitData _patientVisitData;
        private List<PossibleData> _possibleData;
        private double _result;
        private ClassificationModel _selectedClassificationModel;
        private double _possibleResult;

        #endregion


        #region Auto-properties

        public List<ClassificationModel> AvailableClassificationModels { get; set; }
        public PatientPropertyProvider PatientPropertyProvider { get; set; }
        public PatientClassificatorFactory PatientClassificatorFactory { get; set; }
     //   public PatientClassificator PatientClassificator { get; set; }
        public ICommand ClassifyPatientCommand { get; private set; }
        public ICommand ClassifyPossiblePatientCommand { get; private set; }
        public Patient PossiblePatient { get; set; }

        #endregion


        #region Properties

        public List<PossibleData> PossibleData
        {
            get { return _possibleData; }
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
            get { return _selectedClassificationModel; }
            set
            {
                if ( Equals( value, _selectedClassificationModel ) )
                    return;
                _selectedClassificationModel = value;

                PossiblePatient = Mapper.Map<Patient>(Patient);
                PossibleData = new List<PossibleData>
                {
                    new PossibleData
                    {
                        Key = "Возраст",
                        Value1 = PatientPropertyProvider.GetPropertyValue( "Age", PossiblePatient, 
                            PossiblePatient.PatientVisitDataHistory.OrderByDescending(d => d.VisitDate).First() ),
                        Value2 = PatientPropertyProvider.GetPropertyValue( "Age", PossiblePatient,
                            PossiblePatient.PatientVisitDataHistory.OrderByDescending(d => d.VisitDate).First() ),

                    },
                    new PossibleData
                    {
                        Key = "Объем талии",
                        Value1 = PatientPropertyProvider.GetPropertyValue( "{PatientVisitData}.WaistCircumference", PossiblePatient,
                            PossiblePatient.PatientVisitDataHistory.OrderByDescending(d => d.VisitDate).First() ),
                        Value2 =  PatientPropertyProvider.GetPropertyValue( "{PatientVisitData}.WaistCircumference", PossiblePatient,
                            PossiblePatient.PatientVisitDataHistory.OrderByDescending(d => d.VisitDate).First() )
                    },
                    new PossibleData
                    {
                        Key = "Вес",
                        Value1 =  PatientPropertyProvider.GetPropertyValue( "{PatientVisitData}.Weight", PossiblePatient,
                            PossiblePatient.PatientVisitDataHistory.OrderByDescending(d => d.VisitDate).First() ),
                        Value2 = PatientPropertyProvider.GetPropertyValue( "{PatientVisitData}.Weight", PossiblePatient,
                            PossiblePatient.PatientVisitDataHistory.OrderByDescending(d => d.VisitDate).First() )
                    },
                    new PossibleData { Key = "Физ активность",
                        Value1 =  PatientPropertyProvider.GetPropertyValue( "{PatientVisitData}.PhysicalActivity", PossiblePatient,
                                             PossiblePatient.PatientVisitDataHistory.OrderByDescending(d => d.VisitDate).First() ),
                        Value2 =  PatientPropertyProvider.GetPropertyValue( "{PatientVisitData}.PhysicalActivity", PossiblePatient,
                                             PossiblePatient.PatientVisitDataHistory.OrderByDescending(d => d.VisitDate).First() ) }
                };

                OnPropertyChanged();
            }
        }
        

        public double Result
        {
            get { return _result; }
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
            get { return _possibleResult; }
            set
            {
                if (value.Equals(_possibleResult))
                    return;
                _possibleResult = value;
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

        public PatientVisitData PatientVisitData
        {
            get { return _patientVisitData; }
            set
            {
                if ( Equals( value, _patientVisitData ) )
                    return;
                _patientVisitData = value;
                OnPropertyChanged();

                using ( var db = _dbContextFactory.GetDbContext() )
                {
                    AvailableClassificationModels = db.ClassificationModels
                        .Include(model => model.LimitPoints)
                        .Include(model => model.Properties.Select(property => property.Entries))
                        .ToList()
                        .Where(m => IsApplicable(Patient, PatientVisitData, m))
                        .ToList();
                }
            }
        }

        #endregion


        #region Initialization

        public ClassificationTunningViewModel( DbContextFactory dbContextFactory,
                                               PatientPropertyProvider patientPropertyProvider,
                                               PatientClassificatorFactory patientClassificatorFactory )
        {
            PatientPropertyProvider = patientPropertyProvider;
            PatientClassificatorFactory = patientClassificatorFactory;
            _dbContextFactory = dbContextFactory;
            PossibleData = new List<PossibleData>();
            ClassifyPatientCommand = new AsyncDelegateCommand( ClassifyPatientCommandHandler );
            ClassifyPossiblePatientCommand = new AsyncDelegateCommand( ClassifyPossiblePatientCommandHandler );
        }

        #endregion


        #region Non-public methods

        private void ClassifyPatientCommandHandler(object obj )
        {
            var patientClassificator = PatientClassificatorFactory.GetClassificator(SelectedClassificationModel);
            Result = patientClassificator.Classify(Patient, Patient.PatientVisitDataHistory.OrderByDescending(pvd => pvd.VisitDate).First());
           
        }
        private void ClassifyPossiblePatientCommandHandler(object obj)
        {
            var patientClassificator = PatientClassificatorFactory.GetClassificator(SelectedClassificationModel);
            PossibleResult = patientClassificator.Classify(PossiblePatient, PossiblePatient.PatientVisitDataHistory.OrderByDescending(pvd => pvd.VisitDate).First());
        }
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
        private object _value2;
        #region Auto-properties
        private Patient _patient;
        private string _property;
        public string Key { get; set; }
        public object Value1 { get; set; }

        public object Value2
        {
            get { return _value2; }
            set
            {
                _value2 = value;
                
            }
        }

        #endregion
    }
}
