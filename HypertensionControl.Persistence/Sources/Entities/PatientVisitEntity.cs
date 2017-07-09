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
        public Guid PatientId { get; set; }

        [Key]
        [Index]
        [Column( Order = 1 )]
        public long VisitDateTicks { get; set; }

        public PatientEntity Patient { get; set; }

        public int? DepressionPointsCesD { get; set; }
        public int? StressPointsPsm25 { get; set; }
        public double TemporaryBmi { get; set; }
        public double WaistCircumference { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }

        public AlcoholСonsumption AlcoholСonsumption { get; set; }
        public HypertensionStage? HypertensionStage { get; set; }
        public PhysicalActivity? PhysicalActivity { get; set; }

        public string SaltSensitivitySerialized { get; set; }
        public string SmokingSeralized { get; set; }
        public string BloodPressureSerialized { get; set; }
        public string DietaryHabitsSerialized { get; set; }

        #endregion
    }
}
