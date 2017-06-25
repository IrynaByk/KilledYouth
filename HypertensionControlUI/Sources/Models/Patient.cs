using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace HypertensionControlUI.Models
{
    /// <summary>
    ///     Existing genders.
    /// </summary>
    public enum GenderType
    {
        Female,
        Male
    }

    /// <summary>
    ///     Existing hypertension stages.
    /// </summary>
    public enum HypertensionStage
    {
        Healthy,
        Stage1,
        Stage2,
        Stage3
    }

    /// <summary>
    ///     Describes a patient stored in a DB.
    /// </summary>
    public class Patient
    {
        #region Auto-properties

        public int Id { get; set; }

        public string AccompanyingIllnesses { get; set; }
        public string Address { get; set; }
        public long BirthDateTicks { get; set; }
        public string BirthPlace { get; set; }
        public virtual Clinic Clinic { get; set; }
        public string CreatedBy { get; set; }
        public string Diagnosis { get; set; }
        public bool FemaleHeredity { get; set; }
        public GenderType Gender { get; set; }

        public virtual ICollection<Gene> Genes { get; set; }

        public HypertensionAncestralAnamnesis HypertensionAncestralAnamnesis { get; set; }
        public double HypertensionDuration { get; set; }

        public bool MaleHeredity { get; set; }

        public virtual ICollection<Medicine> Medicine { get; set; }
        public string MiddleName { get; set; }

        public string Name { get; set; }
        public string Nationality { get; set; }
        public virtual ICollection<PatientVisitData> PatientVisitHistory { get; set; }
        public string Phone { get; set; }
        public string Surname { get; set; }
        public double? TreatmentDuration { get; set; }

        #endregion


        #region Properties

        [NotMapped]
        public int Age
        {
            get => (DateTime.Now - BirthDate).Days / 365;
            set => BirthDate = DateTime.Now - TimeSpan.FromDays( value * 365 );
        }

        [NotMapped]
        public int? AGT_AGTR2
        {
            get
            {
                int? agt = null;
                int? agtr2 = null;
                foreach ( var gene in Genes )
                {
                    if ( gene.Name.Equals( "AGT" ) )
                        agt = gene.Value;
                    if ( gene.Name.Equals( "AGTR2" ) )
                        agtr2 = gene.Value;
                }
                if ( agt == null || agtr2 == null )
                    return null;
                if ( agtr2 == 3 && agt >= 2 )
                    return 1;
                return 0;
            }
        }

        [NotMapped]
        public DateTime BirthDate
        {
            get => new DateTime( BirthDateTicks );
            set => BirthDateTicks = value.Ticks;
        }

        [NotMapped]
        public PatientVisitData LastVisitData => PatientVisitHistory.OrderByDescending( pvd => pvd.VisitDate ).First();

        [NotMapped]
        public HypertensionStage? HypertensionStage => LastVisitData.HypertensionStage;

        #endregion


        #region Initialization

        public Patient()
        {
            PatientVisitHistory = new List<PatientVisitData>();
            Medicine = new List<Medicine>();
        }

        #endregion
    }

    public class PatientVisitData
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

        public PatientVisitData()
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
