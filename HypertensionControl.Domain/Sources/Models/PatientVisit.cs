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

        public int? DepressionPointsCesD { get; set; }
        public int? StressPointsPsm25 { get; set; }

        public double WaistCircumference { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }

        public AlcoholÑonsumption AlcoholÑonsumption { get; set; }
        public HypertensionStage? HypertensionStage { get; set; }
        public PhysicalActivity? PhysicalActivity { get; set; }

        public SaltSensitivityTest SaltSensitivity { get; set; }
        public Smoking Smoking { get; set; }
        public BloodPressure BloodPressure { get; set; }
        public DietaryHabits DietaryHabits { get; set; }

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
