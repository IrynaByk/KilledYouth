using System;
using System.ComponentModel.DataAnnotations.Schema;
using HypertensionControlUI.Models;

namespace HypertensionControlUI.Persistence.Entities
{
    public class PatientVisitDataEntity
    {
        #region Auto-properties

        public AlcoholСonsumption AlcoholСonsumption { get; set; }
        public BloodPressure BloodPressure { get; set; }
        public int? DepressionPointsCES_D { get; set; }
        public DietaryHabits DietaryHabits { get; set; }
        public double Height { get; set; }
        public HypertensionStage? HypertensionStage { get; set; }

        public int Id { get; set; }
        public virtual Patient Patient { get; set; }
        public PhysicalActivity? PhysicalActivity { get; set; }
        public SaltSensitivityTest SaltSensitivity { get; set; }
        public Smoking Smoking { get; set; }
        public int? StressPointsPSM_25 { get; set; }
        public double TemporaryBMI { get; set; }
        public long VisitDateTicks { get; set; }
        public double WaistCircumference { get; set; }
        public double Weight { get; set; }

        #endregion


        #region Properties

        [NotMapped]
        public double? BMI
        {
            get
            {
                if ( Weight != 0 && Height != 0 )
                    return Weight / Height / Height * 10000;
                if ( TemporaryBMI > 0 )
                    return TemporaryBMI;
                return null;
            }
        }

        [NotMapped]
        public bool? ObesityBMI => BMI != null ? (bool?) (BMI >= 30) : null;

        [NotMapped]
        public bool? ObesityWaistCircumference
        {
            get
            {
                if ( WaistCircumference > 0 )
                {
                    if ( Patient.Gender == 0 )
                        return WaistCircumference > 88;

                    return WaistCircumference > 102;
                }
                return null;
            }
        }

        [NotMapped]
        public DateTime VisitDate
        {
            get => new DateTime( VisitDateTicks );
            set => VisitDateTicks = value.Ticks;
        }

        #endregion


        #region Initialization

        public PatientVisitDataEntity()
        {
            VisitDate = DateTime.Today;
            Smoking = new Smoking();
            DietaryHabits = new DietaryHabits { TesDate = DateTime.Today };
            BloodPressure = new BloodPressure();
            SaltSensitivity = new SaltSensitivityTest { TestDate = DateTime.Today };
        }

        #endregion
    }
}