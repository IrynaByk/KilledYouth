using System;
using HypertensionControl.Domain.Models.Values;

namespace HypertensionControl.Domain.Models
{
    public class PatientVisit
    {
        #region Auto-properties

        /// <summary>
        ///     Unique patient visit identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Backreference to the patient.
        /// </summary>
        public Patient Patient { get; set; }

        /// <summary>
        ///     Visit date.
        /// </summary>
        public DateTime VisitDate { get; set; }

        public int DepressionPointsCesD { get; set; }
        public int StressPointsPsm25 { get; set; }

        public double WaistCircumference { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }

        public AlcoholСonsumption AlcoholСonsumption { get; set; }
        public HypertensionStage? HypertensionStage { get; set; }
        public PhysicalActivity? PhysicalActivity { get; set; }
        public int FruitVegInDailyDiet { get; set; }

        public SaltSensitivityTest SaltSensitivity { get; set; }
        public Smoking Smoking { get; set; }
        public BloodPressure BloodPressure { get; set; }
        public DietaryHabits DietaryHabits { get; set; }
       
        public DateTime? ElectrocardiogramDate { get; set; }
        public double PulsWaveVelocity { get; set; }
        public double AugmentationIndex { get; set; }
        public DateTime? DailyMonitoringOfBloodPressureDate { get; set; }
        public double TotalCholesterol { get; set; }
        public double Glucose { get; set; }
        public double GlycolicHemoglobin { get; set; }
        public double Creatinine { get; set; } 
        public double ScoreRisk { get; set; } 
        //скрининг фибрилляции предсердия
        public bool AtrialFibrillationScreening { get; set; }
        //сердечно-лодыжечный сосудистый индекс 
        public double CardiovascularVascularIndexRight { get; set; }
        public double CardiovascularVascularIndexLeft { get; set; }
        //лодыжечно-плечевой индекс давления
        public double AnkleBrachialPressureIndexRight { get; set; }
        public double AnkleBrachialPressureIndexLeft { get; set; }
        //биологический возраст магистральных артерий
        public double BiologicalArteriesAgeRightMin { get; set; }
        public double BiologicalArteriesAgeRightMax { get; set; }
        public double BiologicalArteriesAgeLeftMin { get; set; }
        public double BiologicalArteriesAgeLeftMax { get; set; }


        #endregion


        #region Properties

        public double? Bmi
        {
            get
            {
                if ( Weight > 0 && Height > 0 )
                    return Weight / Height / Height * 10000;
                return null;
            }
        }

        public double? SmokeIndex => Smoking.Type.Equals( SmokingType.Never ) ? null : (double?)(Smoking.DurationInYears * Smoking.CigarettesPerDay / 20.0);
        public bool? ObesityBmi => Bmi != null ? Bmi >= 30 : (bool?) null;

        public bool? ObesityWaistCircumference
        {
            get
            {
                if ( WaistCircumference <= 0 )
                    return null;

                return Patient.Gender == GenderType.Female
                    ? WaistCircumference > 88
                    : WaistCircumference > 102;
            }
        }

        #endregion


        #region Initialization

        private PatientVisit()
        {
        }

        #endregion


        #region Public methods

        public static PatientVisit CreateTodayVisit( Patient patient )
        {
            var visit = new PatientVisit
            {
                Patient = patient,
                VisitDate = DateTime.Today,
                Smoking = new Smoking(),
                DietaryHabits = new DietaryHabits { TestDate = DateTime.Today },
                BloodPressure = new BloodPressure(),
                SaltSensitivity = new SaltSensitivityTest { TestDate = DateTime.Today }
            };

            return visit;
        }

        #endregion
    }
}
