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
    public class PatientStatisticsViewModel : PageViewModelBase
    {
        #region Fields

        private readonly DbContextFactory _dbContextFactory;
        private readonly IViewProvider _viewProvider;
        private readonly MainWindowViewModel _mainWindowViewModel;
        private List<Patient> _patients;
        private List<PatientStatisticsRowViewModel> _patientStatisticsData;
        public ICommand PatientsCommand { get; }
        #endregion


        #region Auto-properties

        public List<ClassificationModel> ClassificationModels { get; set; }

        public PatientClassificatorFactory PatientClassificatorFactory { get; set; }

        #endregion

        public List<Patient> Patients
        {
            get => _patients;
            set
            {
                if (_patients == value)
                    return;
                _patients = value;
            }
        }

        public List<PatientStatisticsRowViewModel> PatientStatisticsData
        {
            get => _patientStatisticsData;
            set
            {
                if (Equals(value, _patientStatisticsData))
                    return;
                _patientStatisticsData = value;
                OnPropertyChanged();
            }
        }
        #region Initialization

        public PatientStatisticsViewModel( DbContextFactory dbContextFactory,
                                               PatientClassificatorFactory patientClassificatorFactory,
                                           IViewProvider viewProvider,
                                           MainWindowViewModel mainWindowViewModel )
        {
            PatientClassificatorFactory = patientClassificatorFactory;
            _viewProvider = viewProvider;
            _mainWindowViewModel = mainWindowViewModel;
            _dbContextFactory = dbContextFactory;
            using (var db = _dbContextFactory.GetDbContext())
            {
                Patients = db.Patients
                             .Include(p => p.PatientVisitDataHistory)
                             .Include(p => p.Clinic)
                             .Include(p => p.Medicine)
                             .Include(p => p.Genes)
                             .ToList();
                ClassificationModels = db.ClassificationModels
                                         .Include( model => model.LimitPoints )
                                         .Include( model => model.Properties.Select( property => property.Entries ) )
                                         .ToList();
            }
            PatientStatisticsData = new List<PatientStatisticsRowViewModel>();
            foreach ( var patient in Patients )
            {
                var patientSatisticsRowData = new PatientStatisticsRowViewModel(patient, ClassificationModels[0],ClassificationModels[1]);
                PatientStatisticsData.Add(patientSatisticsRowData);
            }
            PatientsCommand = new AsyncDelegateCommand(o => _viewProvider.NavigateToPage<PatientsViewModel>(m => _mainWindowViewModel.Patient = null));


        }

        #endregion


        #region Non-public methods



        #endregion
    }

    public class PatientStatisticsRowViewModel : ViewModelBase
    {
        private readonly Patient _patient;
        private readonly ClassificationModel _modelWithGene;
        private readonly ClassificationModel _modelWithoutGene;
        private double _cutOff;
        

        public PatientStatisticsRowViewModel(Patient patient, ClassificationModel classificationModel1, ClassificationModel classificationModel2)
        {
            _patient = patient;
            _modelWithGene = classificationModel1;
            _modelWithoutGene = classificationModel2;
        }

        public double CutOff
        {
            get => _cutOff;
            set
            {
                if (value == _cutOff)
                    return;
                _cutOff = value;
                OnPropertyChanged(nameof(ModelWithGeneLastResult));
            }
        }

        public double? ModelWithGeneLastResult
        {
            get
            {
                var patientVisitData = _patient.PatientVisitDataHistory
                                  .OrderByDescending( d => d.VisitDate )
                                  .First();
                if (!IsApplicable(_patient, patientVisitData, _modelWithGene))
                {
                    return null;
                }
                return Score(patientVisitData, _modelWithGene);
            }
        }

        public double? ModelWithoutGeneLastResult
        {
            get
            {
                var patientVisitData = _patient.PatientVisitDataHistory
                                              .OrderByDescending(d => d.VisitDate)
                                              .First();
                if (!IsApplicable(_patient, patientVisitData, _modelWithoutGene))
                {
                    return null;
                }
                return Score(patientVisitData, _modelWithoutGene);
            }
        }

        public string Name => _patient.Surname + " " + _patient.Name + " " + _patient.MiddleName;
        public HypertensionStage? LastStage =>_patient.Stage;
        public string ModelPrediction(PatientVisitData patientVisitData, ClassificationModel model)
        {
            if ( !IsApplicable( _patient, patientVisitData, model) )
            {
                return "нет данных";
            }
            var score = Score( patientVisitData, model );
            string result = score.ToString( "F4" ) + "/";

            if ( score > CutOff )
            {
                result += "болен";
            }
            else
            {
                result += "здоров";
            }
            return result;
        }
         
        private double Score( PatientVisitData patientVisitData, ClassificationModel model )
        {
            var classificator = new PatientClassificator( model );
            var score = classificator.Classify( new {Patient =  _patient, PatientVisitData = patientVisitData });
            return score;
        }

        private bool IsApplicable(Patient patient, PatientVisitData visitData, ClassificationModel classificationModel)
        {
            return classificationModel
                .Properties
                .All(p => PatientPropertyProvider.GetPropertyValue(p.Name, new { Patient= patient, PatientVisistData = visitData}) != null);
        }
    }


}
