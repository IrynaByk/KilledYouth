using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Input;
using HypertensionControl.Domain.Models;
using HypertensionControl.Domain.Models.Values;
using HypertensionControlUI.Interfaces;
using HypertensionControlUI.Services;
using HypertensionControlUI.Utils;
using HypertensionControlUI.ViewModels;

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

        public bool HasClassificationResult => ClassificationResult > 0;
        public bool HasPossibleClassificationResult => PossibleClassificationResult != null && PossibleClassificationResult > 0;
        public double? ClassificationResult => _mainWindowViewModel.ClassificationResult;
        public double? ClassificationResultPercent => ClassificationResult * 100;
        public double? PossibleClassificationResult => _mainWindowViewModel.PossibleClassificationResult;
        public string ClinicName => _mainWindowViewModel.User.ClinicName;
        public string ClinicAddress => _mainWindowViewModel.User.ClinicAddress;

        public PatientVisit PatientVisit
        {
            get => _patientVisit;
            set => Set( ref _patientVisit, value );
        }

        public string AtrialFibrillationScreening
        {
            get => PatientVisit.AtrialFibrillationScreening ? "есть" : "нет";
        }

        public Patient Patient
        {
            get => _patient;
            set => Set( ref _patient, value );
        }

        public string PossibleChangesReport
        {
            get
            {
                if (!HasPossibleClassificationResult)
                return "";
                StringBuilder result = new StringBuilder("При нормализации");
                if ( _mainWindowViewModel.CorrectedPatient.LastVisit.Weight != _mainWindowViewModel.Patient.LastVisit.Weight )
                {
                    result.Append( " веса до " ).Append( _mainWindowViewModel.CorrectedPatient.LastVisit.Weight ).Append( " кг, " );
                }
                if (_mainWindowViewModel.CorrectedPatient.LastVisit.WaistCircumference != _mainWindowViewModel.Patient.LastVisit.WaistCircumference)
                    result.Append(" окружности талии до ").Append(_mainWindowViewModel.CorrectedPatient.LastVisit.WaistCircumference).Append(" см,") ;
                if ( _mainWindowViewModel.CorrectedPatient.LastVisit.PhysicalActivity != _mainWindowViewModel.Patient.LastVisit.PhysicalActivity )
                {
                    string phizNumber = "";
                    switch (_mainWindowViewModel.CorrectedPatient.LastVisit.PhysicalActivity)
                    {
                        case PhysicalActivity.OncePerWeekOrLess:
                            phizNumber = "до одного раза";
                            break;
                        case PhysicalActivity.FromOneToThreeTimesPerWeek:
                            phizNumber = "от одного до трёх раз";
                            break;
                        case PhysicalActivity.MoreThenThreeTimesPerWeek:
                            phizNumber = "до трёх и более раз";
                            break;
                            
                    }
                    result.Append(" аэробной физической активности ").Append( phizNumber).Append(" в неделю длительностью более 30-40 минут");
                }
                result.Append(" в течение ").Append(_mainWindowViewModel.CorrectedPatient.Age - _mainWindowViewModel.Patient.Age)
                    .Append(  " лет Ваш индивидуальный риск развития артериальной гипертензии снизится на ")
                    .Append((_mainWindowViewModel.ClassificationResult - _mainWindowViewModel.PossibleClassificationResult)?.ToString("P2", CultureInfo.InvariantCulture));
                return result.ToString();
            }
        }

        public string IndividualRiskFactorsReport
        {
            get
            {
                StringBuilder result = new StringBuilder();
                if ( !Patient.HypertensionAncestralAnamnesis.Equals( HypertensionAncestralAnamnesis.None ) )
                    result.Append("Отягощенная наследственность по развитию  ранних сердечно-сосудистых осложнений.\r\n");
                if ( Patient.MaleHeredity || Patient.FemaleHeredity )
                    result.Append("Отягощенная наследственность по развитию артериальной гипертензии.\r\n");
                if ( Patient.LastVisit.Bmi > 25 && Patient.LastVisit.Bmi < 30)
                    result.Append("Избыточный вес");
                else if ( Patient.LastVisit.Bmi >= 30 && Patient.LastVisit.Bmi < 35)
                    result.Append("Ожирение первой степени");
                else if (Patient.LastVisit.Bmi >= 35 && Patient.LastVisit.Bmi < 40)
                    result.Append("Ожирение второй степени");
                else if (Patient.LastVisit.Bmi >= 40)
                    result.Append("Ожирение третьей степени");
                if ( Patient.LastVisit.ObesityWaistCircumference.HasValue && Patient.LastVisit.ObesityWaistCircumference.Value )
                    result.Append(", в том числе – абдоминальное ожирение.\r\n");
                else if (Patient.LastVisit.Bmi >= 30)
                    result.Append(".\r\n");
                if (Patient.LastVisit.BloodPressure != null && Patient.LastVisit.BloodPressure.RightShoulderSbp > 140 && Patient.LastVisit.BloodPressure.RightShoulderDbp > 90 )
                    result.Append("Повышенный уровень АД. ");
                if ( PatientVisit.SmokeIndex > 10 )
                    result.Append( "Риск развития ХОБЛ. " );
                if (Patient.LastVisit.TotalCholesterol > 5)
                    result.Append("Гиперхолестеринемия. ");
                if (Patient.LastVisit.Glucose > 5.6 && Patient.LastVisit.Glucose <= 6)
                    result.Append("Нарушенная гликемия натощак. ");
                else if (Patient.LastVisit.Glucose > 6)
                    result.Append("Сахарный диабет? Необходимо повторить анализ. ");
                if ( Patient.LastVisit.StressPointsPsm25 >= 100 && Patient.LastVisit.StressPointsPsm25 <= 154 )
                    result.Append("\r\nСредний уровень психической напряженности и снижение стрессоустойчивости.");
                else if ( Patient.LastVisit.StressPointsPsm25 > 154 )
                    result.Append("\r\nВысокий уровень нервно-психической напряженности и дезадаптация к стрессу.");
                if (Patient.LastVisit.DepressionPointsCesD >= 19 && Patient.LastVisit.DepressionPointsCesD <= 25)
                    result.Append("\r\nПризнаки легкого тревожно-депрессивного расстройства.");
                else if (Patient.LastVisit.DepressionPointsCesD > 25)
                    result.Append("\r\nПризнаки тревожно-депрессивного расстройства, рекоммендуется консультация психотерапевта.");
                if ( Patient.LastVisit.ScoreRisk >= 10 )
                    result.Append("\r\nСердечно-сосудистый риск очень высокий по шкале SCORE. ");
                else if (Patient.LastVisit.ScoreRisk >= 5 && Patient.LastVisit.ScoreRisk < 10)
                    result.Append("\r\nСердечно-сосудистый риск высокий по шкале SCORE. ");
                else if (Patient.LastVisit.ScoreRisk >= 1 && Patient.LastVisit.ScoreRisk < 5)
                    result.Append("\r\nСердечно-сосудистый риск умеренный по шкале SCORE. ");
                if ( Patient.LastVisit.CardiovascularVascularIndexRight >= 9.0 || Patient.LastVisit.CardiovascularVascularIndexLeft >= 9.0 )
                    result.Append("Увеличение индекса жесткости артерий. ");
                if ( Patient.LastVisit.AnkleBrachialPressureIndexRight > 1.3 || Patient.LastVisit.AnkleBrachialPressureIndexLeft > 1.3 )
                    result.Append("Кальциноз артерий нижних конечностей? ");
                else if (Patient.LastVisit.AnkleBrachialPressureIndexRight <= 0.9 || Patient.LastVisit.AnkleBrachialPressureIndexLeft < 0.9)
                    result.Append("Стеноз артерий нижних конечностей? ");
                if (Patient.LastVisit.BiologicalArteriesAgeRightMax > Patient.Age ||
                    Patient.LastVisit.BiologicalArteriesAgeLeftMax > Patient.Age)
                    result.Append("Биологический возраст сосудов превышает календарный возраст пациента.");
                else  result.Append("Биологический возраст сосудов соответствует календарному возраст пациента.");

                return result.ToString();
            }
        }

        public string Diet
        {
            get
            {
                if ( PatientVisit.FruitVegInDailyDiet < 400 )
                    return " менее 400 грамм овощей и фруктов в день.";
                return " более 400 грамм овощей и фруктов в день.";

            }
        }

        public string PatientHeredity
        {
            get
            {
                if (Patient.MaleHeredity && Patient.FemaleHeredity)
                    return "отягощён (оба родителя)";
                if (!Patient.MaleHeredity && !Patient.FemaleHeredity)
                    return "не отягощён";
                if (Patient.MaleHeredity)
                    return "отягощён (отец)";
                return "отягощён (мать)";
            }
        }
        public string HypertensionAnamnesis
        {
            get
            {
                if (Patient.HypertensionAncestralAnamnesis.Equals(HypertensionAncestralAnamnesis.BothMotherAndFather))
                    return "отягощён (оба родителя)";
                if (Patient.HypertensionAncestralAnamnesis.Equals(HypertensionAncestralAnamnesis.None))
                    return "не отягощён";
                if (Patient.HypertensionAncestralAnamnesis.Equals(HypertensionAncestralAnamnesis.Father))
                    return "отягощён (отец)";
                return "отягощён (мать)";
            }
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
        
        public string Alcohol
        {
            get
            {
                if (Patient.LastVisit.AlcoholСonsumption.Equals(AlcoholСonsumption.Never))
                    return "никогда или редко";
                if (Patient.LastVisit.AlcoholСonsumption.Equals(AlcoholСonsumption.Never))
                    return "1-3 дозы в месяц";
                if (Patient.LastVisit.AlcoholСonsumption.Equals(AlcoholСonsumption.Never))
                    return "1-6 доз в неделю";
                return "больше 1 дозы в день";
            }
        }
        public string Phiz
        {
            get
            {
                if (Patient.LastVisit.PhysicalActivity.Equals(PhysicalActivity.Never))
                    return "редко";
                if (Patient.LastVisit.PhysicalActivity.Equals(PhysicalActivity.OncePerWeekOrLess))
                    return "один раз в неделю или меньше";
                if (Patient.LastVisit.PhysicalActivity.Equals(PhysicalActivity.FromOneToThreeTimesPerWeek))
                    return "один-три раза в неделю";
                return "больше трёх раз в неделю";
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
        public string SmokeIndexText
        {
            get
            {
                var smokeIndex = PatientVisit.SmokeIndex;
                return smokeIndex == null ||  smokeIndex < 0.01
                    ? ""
                    : $"индекс курильщика  – {smokeIndex:f2}";
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

        public IndividualPatientCardViewModel( IViewProvider viewProvider, MainWindowViewModel mainWindowViewModel)
        {
            _viewProvider = viewProvider;
            _mainWindowViewModel = mainWindowViewModel;
            ClassifyPatientTunningCommand = new AsyncDelegateCommand( ClassifyPatientTunningCommandHandler );
            PatientsCommand = new AsyncDelegateCommand( o =>
            {
                _viewProvider.NavigateToPage<PatientsViewModel>( patientsViewModel =>
                {
                    patientsViewModel.SelectedPatient = patientsViewModel.Patients.Find( p => p.Id == Patient.Id );
                    patientsViewModel.SelectedVisit = patientsViewModel.SelectedPatient?.VisitHistory.FirstOrDefault( v => v.VisitDate == PatientVisit.VisitDate );
                } );
            } );
        }

        #endregion
    }
}
