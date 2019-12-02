using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Windows.Input;
using AutoMapper;
using HypertensionControl.Domain.Models;
using HypertensionControl.Domain.Models.Values;
using HypertensionControl.Persistence.Services;
using HypertensionControlUI.Annotations;
using HypertensionControlUI.CompositionRoot;
using HypertensionControlUI.Interfaces;
using HypertensionControlUI.Services;
using HypertensionControlUI.Utils;

namespace HypertensionControlUI.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged, IPageViewModel
    {
        #region Fields

        private readonly IdentityService _identityService;

        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly IViewProvider _viewProvider;
        private bool _loginFailed;

        #endregion


        #region Auto-properties

        public string Login { get; set; }
        public string Password { get; set; }
        public ICommand LoginCommand { get; }

        #endregion


        #region Properties

        public bool LoginFailed
        {
            get => _loginFailed;
            set
            {
                if ( value == _loginFailed )
                    return;
                _loginFailed = value;
                OnPropertyChanged();
            }
        }

        #endregion


        #region Commands

        private void LoginCommandHandler( object o )
        {
            try
            {
                LoginFailed = false;
                _identityService.Login( Login, Password );
                _mainWindowViewModel.User = _identityService.CurrentUser;
                _viewProvider.NavigateToPage<PatientsViewModel>();
            }
            catch ( AuthenticationException )
            {
                LoginFailed = true;
            }
        }

        private bool LoginCommandCanExecute( object o )
        {
            return !string.IsNullOrEmpty( Login ) && !string.IsNullOrEmpty( Password );
        }

        #endregion


        #region Events and invocation

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }

        #endregion


        #region Initialization

        public LoginViewModel( MainWindowViewModel mainWindowViewModel,
                               IdentityService identityService,
                               IViewProvider viewProvider,
                               IUnitOfWorkFactory factory,
                               PatientClassificatorFactory patientClassificatorFactory
                               )
        {
            using ( var uow = factory.CreateUnitOfWork() )
            {
                var model = uow.ClassificationModelsRepository.GetAllClassificationModels()
                               .First( m => m.Name == "Классификационная модель не требующая генетических данных." );
                var patients = uow.PatientsRepository.GetAllPatients().Where( p => p.RegisteredBy == "InnaViktorovna").ToList();
                var mm = patients.Count( p => p.Gender == GenderType.Male );
                var ff = patients.Count( p => p.Gender == GenderType.Female );
                //           + 1.повышенное АД
                //           + 2.гиперхолестеринемия
                //           + 3.низкая физическая активность
                //          +  4.Избыточный вес и ожирение.
                //          +  5.Висцеральное ожирение.
                //          +  6.Увеличение индекса жесткости артерий.
                //           + 7.Сердечно - сосудистый риск по шкале SCORE 5 и более%.
                //          +  8.биологический возраст сосудов превышает календарный возраст пациента.
                //1 наследственность по ранним сердечно-сосудистым осложнениям
                var totalHeredity = patients.Count( p => p.FemaleHeredity || p.MaleHeredity );
                //2 наследственность по АГ
                var hHeredity = patients.Count(p => !p.HypertensionAncestralAnamnesis.Equals( HypertensionAncestralAnamnesis.None ));
                //ИМТ от 25 до 29,9
                var BMIHigh1 = patients.Count( p => p.LastVisit.Bmi < 30 && p.LastVisit.Bmi >= 25);
//                4.ИМТ от 30 и более
                var BMIHigh2 = patients.Count( p => p.LastVisit.Bmi >= 30);
                //5. окружность талии более 94 см у мужчин и более 80 см у женщин
                var waist = patients.Count( p => p.Gender == GenderType.Female
                 ? p.LastVisit.WaistCircumference > 80 : p.LastVisit.WaistCircumference > 94);
                //6. АД равно или более 140/90 мм рт.ст
                var bloodPressureHigh = patients.Count(p => p.LastVisit.BloodPressure != null && p.LastVisit.BloodPressure.RightShoulderSbp >= 140 &&
                                                            p.LastVisit.BloodPressure.RightShoulderDbp >= 90);
                //7. превышение общего холестерина более 5 ммоль/л
                var cholesterolHigh = patients.Count(p => p.LastVisit.TotalCholesterol > 5);
                //8.превышение глюкозы капилл крови, если 5,6- 6,0 ммоль/л и ,если 6,1 и выше
                var glucose1 = patients.Count(p => p.LastVisit.Glucose >= 5.6 && p.LastVisit.Glucose <= 6.0);
                var glucose2 = patients.Count(p => p.LastVisit.Glucose > 6.0);

                //9.Курение
                var smokers = patients.Count(p => p.LastVisit.Smoking.Type == SmokingType.Now);

                //10.индекс курильщика
                var smokerIndex = patients.Count(p => p.LastVisit.SmokeIndex >= 10);

                //11.физическая активность менее 1 раза в неделю или редко - "низкая физическая активность", 
                var phizLow = patients.Count(p => p.LastVisit.PhysicalActivity >= PhysicalActivity.OncePerWeekOrLess);

                // 12.в рационе питания – менее 500 грамм в день овощей и фруктов -"недостаточное потребление овощей и фруктов
                var fruit = patients.Count(p => p.LastVisit.FruitVegInDailyDiet < 500);

                // 13.шкала CES - D) -от 19 баллов до 25 баллов -«Признаки легкого тревожно - депрессивного расстройства»,
                // если 26 и более  -«Признаки тревожно-депрессивного расстройства"
                var ces1 = patients.Count( p => p.LastVisit.DepressionPointsCesD >= 19 && p.LastVisit.DepressionPointsCesD <= 25);
                var ces2 = patients.Count( p => p.LastVisit.DepressionPointsCesD >= 26 );
                // 14.Алкоголь по ранжированию
                var alco1 = patients.Count(p => p.LastVisit.AlcoholСonsumption.Equals( AlcoholСonsumption.Never ));
                var alco2 = patients.Count(p => p.LastVisit.AlcoholСonsumption.Equals( AlcoholСonsumption.Monthly ));
                var alco3 = patients.Count(p => p.LastVisit.AlcoholСonsumption.Equals( AlcoholСonsumption.Weekly ));
                var alco4 = patients.Count(p => p.LastVisit.AlcoholСonsumption.Equals( AlcoholСonsumption.Daily ));
                //15. Шкала PSM-25 - если 100-154 балла -«Средний уровень психической напряженности и снижение стрессоустойчивости», 
               // если равно и более 155 баллов - «Высокий уровень нервно - психической напряженности и дезадаптация к стрессу"
               var psm1 = patients.Count( p => p.LastVisit.StressPointsPsm25 >= 100 && p.LastVisit.StressPointsPsm25 <= 154 );
               var psm2 = patients.Count( p => p.LastVisit.StressPointsPsm25 >= 155 );
                //16.Сердечно - лодыжечный сосудистый индекс – если равно 9,0 и более – «Увеличение индекса жесткости артерий»
                var rigidIndex =
                    patients.Count(p => p.LastVisit.CardiovascularVascularIndexRight >= 9.0 || p.LastVisit.CardiovascularVascularIndexLeft >= 9.0);

                // 17.Лодыжечно - плечевой индекс давления -если  менее 0,9 - «Стеноз артерий нижних конечностей?»,
                //и если более 1,3 -  «Кальциноз артерий нижних конечностей?»
                var index1 =
                    patients.Count(p => p.LastVisit.CardiovascularVascularIndexRight < 0.9 
                                        || p.LastVisit.CardiovascularVascularIndexLeft < 0.9);
                var index2 =
                    patients.Count(p => p.LastVisit.CardiovascularVascularIndexRight >= 1.3
                                        || p.LastVisit.CardiovascularVascularIndexLeft >= 1.3);
                //18.Количество человек , у которых биологический возраст сосудов превышает календарный возраст пациента.
                var vesselAge = patients.Count(p => (p.LastVisit.BiologicalArteriesAgeLeftMax > p.Age) ||
                                                    (p.LastVisit.BiologicalArteriesAgeRightMax > p.Age));
                //19.Риск развития АГ: низкий, средний и умеренный.
               
               var patientClassificator = patientClassificatorFactory.GetClassificator(model);
                var low = 0;
                var middle = 0;
                var high = 0;
                foreach ( var p in patients )
                {
                    var risk = patientClassificator.Classify(new { Patient = p, PatientVisit = p.LastVisit });
                    if (risk <= 0.36)
                    {
                        low++;
                    }
                    else if ( risk <= 0.73 )
                    {
                        middle++;
                    }
                    else
                        high++;
                }
                // 20.Риск по шкале SCORE: низкий, средний, высокий, очень высокий.
                var highSCORE1 = patients.Count(p => p.LastVisit.ScoreRisk < 1.0);
                var highSCORE2 = patients.Count(p => p.LastVisit.ScoreRisk >= 1.0 && p.LastVisit.ScoreRisk < 5.0);
                var highSCORE3 = patients.Count(p => p.LastVisit.ScoreRisk >= 5.0 && p.LastVisit.ScoreRisk < 10.0);
                var highSCORE4 = patients.Count(p => p.LastVisit.ScoreRisk >= 10.0);
                var age1 = patients.Count(p => p.Age >= 18.0 && p.Age <= 30);
                var age11 = patients.Where(p => p.Age >= 18.0 && p.Age <= 30).Average(p => p.Age);
                var age2 = patients.Count(p => p.Age > 30.0 && p.Age <= 40);
                var age21 = patients.Where(p => p.Age > 30.0 && p.Age <= 40).Average(p => p.Age);
                var age3 = patients.Count(p => p.Age > 40.0 && p.Age <= 50);
                var age31 = patients.Where(p => p.Age > 40.0 && p.Age <= 50).Average(p => p.Age);
                var age4 = patients.Count(p => p.Age > 50.0 && p.Age <= 60);
                var age41 = patients.Where(p => p.Age > 50.0 && p.Age <= 60).Average(p => p.Age);
                var age5 = patients.Count(p => p.Age > 60.0);
                var age6 = patients.Count(p => p.Age > 70.0);
            }
            _mainWindowViewModel = mainWindowViewModel;
            _identityService = identityService;
            _viewProvider = viewProvider;
            LoginCommand = new AsyncDelegateCommand( LoginCommandHandler, LoginCommandCanExecute );
        }

        #endregion
    }
}
