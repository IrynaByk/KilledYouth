using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HypertensionControl.Domain.Models.Values;

namespace HypertensionControl.Persistence.Entities
{
    /// <summary>
    ///     A single visit of a patient.
    /// </summary>
    [Table( "PatientVisit" )]
    public sealed class PatientVisitEntity
    {
        #region Auto-properties

        [Key]
        [Index]
        [Column( Order = 0 )]
        public string PatientId { get; set; }

        [Key]
        [Index]
        [Column( Order = 1 )]
        public long VisitDateTicks { get; set; }

        public PatientEntity Patient { get; set; }

        public int DepressionPointsCesD { get; set; }
        public int StressPointsPsm25 { get; set; }
        public double WaistCircumference { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }

        public AlcoholСonsumption AlcoholСonsumption { get; set; }
        public HypertensionStage? HypertensionStage { get; set; }
        public PhysicalActivity? PhysicalActivity { get; set; }
        public int FruitVegInDailyDiet { get; set; }

        public string SaltSensitivitySerialized { get; set; }
        public string SmokingSeralized { get; set; }
        public string BloodPressureSerialized { get; set; }
        public string DietaryHabitsSerialized { get; set; }
        public long? ElectrocardiogramDateTicks { get; set; }
        public double PulsWaveVelocity { get; set; }
        public double AugmentationIndex { get; set; }
        public long? DailyMonitoringOfBloodPressureDateTicks { get; set; }
        public double TotalCholesterol { get; set; }
        public double Glucose { get; set; }
        public double GlycolicHemoglobin { get; set; }
        public double Creatinine { get; set; }
        public double ScoreRisk { get; set; }
        public bool AtrialFibrillationScreening { get; set; }
        public double CardiovascularVascularIndexRight { get; set; }
        public double CardiovascularVascularIndexLeft { get; set; }
        public double AnkleBrachialPressureIndexRight { get; set; }
        public double AnkleBrachialPressureIndexLeft { get; set; }
        public double BiologicalArteriesAgeRightMin { get; set; }
        public double BiologicalArteriesAgeRightMax { get; set; }
        public double BiologicalArteriesAgeLeftMin { get; set; }
        public double BiologicalArteriesAgeLeftMax { get; set; }
        #endregion
    }
}
