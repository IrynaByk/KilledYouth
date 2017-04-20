using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows.Input;
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
        public PatientClassificator PatientClassificator { get; set; }
        public ICommand ClassifyPatientCommand { get; private set; }
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
                PossibleData = new List<PossibleData>
                {
                    new PossibleData { Key = "Возраст", Value1 = Patient.Age, Value2 = Patient.Age + 1 },
                    new PossibleData
                    {
                        Key = "Объем талии",
                        Value1 = PatientVisitData.WaistCircumference,
                        Value2 = PatientVisitData.WaistCircumference - 34
                    },
                    new PossibleData
                    {
                        Key = "Вес",
                        Value1 = PatientVisitData.Weight,
                        Value2 = PatientVisitData.Weight - 35
                    },
                    new PossibleData { Key = "Физ активность", Value1 = 2, Value2 = 3 }
                };
                PossiblePatient = new Patient
                {
                    BirthDate = new DateTime(1977, 11, 11),
                    Gender = GenderType.Male,
                    Genes =
                    {
                        new Gene { Name = "AGT", Value = 2 },
                        new Gene { Name = "AGTR2", Value = 3 }
                    }
                };
                var pvd = new PatientVisitData
                {
                    WaistCircumference = 90,
                    Height = 182,
                    Weight = 85,
                    PhysicalActivity = PhysicalActivity.FromOneToThreeTimesPerWeek,
                    Patient = PossiblePatient
                };
                PossiblePatient.PatientVisitDataHistory.Add(pvd);
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
        }

        #endregion


        #region Non-public methods

        private void ClassifyPatientCommandHandler(object obj)
        {
            var patientClassificator = PatientClassificatorFactory.GetClassificator(SelectedClassificationModel);

            Result = patientClassificator.Classify(Patient,Patient.PatientVisitDataHistory.OrderByDescending(pvd => pvd.VisitData).First());
            PossibleResult = patientClassificator.Classify(PossiblePatient,PossiblePatient.PatientVisitDataHistory.OrderByDescending(pvd => pvd.VisitData).First());
        }

        private bool IsApplicable( Patient patient, PatientVisitData visitData, ClassificationModel classificationModel )
        {
            var temp = classificationModel
                .Properties
                .All( p => PatientPropertyProvider.GetPropertyValue( p.Name, patient, visitData ) != null );

            var name = classificationModel.Properties.FirstOrDefault( p => PatientPropertyProvider.GetPropertyValue( p.Name, patient, visitData ) == null );

            return classificationModel
                .Properties
                .All( p => PatientPropertyProvider.GetPropertyValue( p.Name, patient, visitData ) != null );
        }

        #endregion
    }

    public class PossibleData
    {
        #region Auto-properties

        public string Key { get; set; }
        public double Value1 { get; set; }
        public double Value2 { get; set; }

        #endregion
    }
}
