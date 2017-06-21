using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace HypertensionControlUI.Models
{
    public class Patient
    {
        #region Auto-properties

        //        [DatabaseGenerated( DatabaseGeneratedOption.Identity )]
        public int Id { get; set; }

        public string Name { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string CreatedBy { get; set; }
        public long BirthDateTicks { get; set; }
        public string BirthPlace { get; set; }
        public GenderType Gender { get; set; }
        public string Nationality { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Diagnosis { get; set; }
        public string AccompanyingIllnesses { get; set; }
        public virtual Clinic Clinic { get; set; }
        public double HypertensionDuration { get; set; }
        public double? TreatmentDuration { get; set; }
        public bool MaleHeredity { get; set; }
        public bool FemaleHeredity { get; set; }

        public virtual ICollection<Medicine> Medicine { get; set; }

        public HypertensionAncestralAnamnesis HypertensionAncestralAnamnesis { get; set; }

        public virtual ICollection<Gene> Genes { get; set; }
        public virtual ICollection<PatientVisitData> PatientVisitDataHistory { get; set; }

        #endregion


        #region Properties

        [NotMapped]
        public DateTime BirthDate
        {
            get => new DateTime( BirthDateTicks );
            set => BirthDateTicks = value.Ticks;
        }

        [NotMapped]
        public int Age
        {
            get => (DateTime.Now - BirthDate).Days / 365;
            set => BirthDate = DateTime.Now - TimeSpan.FromDays( Age * 365 );
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
        public HypertensionStage? Stage
        {
            get { return PatientVisitDataHistory.OrderByDescending( d => d.VisitDate ).First().HypertensionStage; }
        }

        #endregion


        #region Initialization

        public Patient()
        {
            PatientVisitDataHistory = new List<PatientVisitData>();
//            Genes = new List<Gene>();
            Medicine = new List<Medicine>();
        }

        #endregion
    }

    public enum GenderType
    {
        Female,
        Male
    }

    public enum HypertensionStage
    {
        Healthy,
        Stage1,
        Stage2,
        Stage3
    }

    public class PatientVisitData
    {
        #region Auto-properties

        public int Id { get; set; }
        public long VisitDateTicks { get; set; }
        public AlcoholСonsumption AlcoholСonsumption { get; set; }
        public int? StressPointsPSM_25 { get; set; }
        public int? DepressionPointsCES_D { get; set; }
        public SaltSensitivityTest SaltSensitivity { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public double TemporaryBMI { get; set; }
        public double WaistCircumference { get; set; }
        public Smoking Smoking { get; set; }
        public DietaryHabits DietaryHabits { get; set; }
        public BloodPressure BloodPressure { get; set; }
        public PhysicalActivity? PhysicalActivity { get; set; }
        public HypertensionStage? HypertensionStage { get; set; }
        public virtual Patient Patient { get; set; }

        #endregion


        #region Properties

        [NotMapped]
        public DateTime VisitDate
        {
            get => new DateTime( VisitDateTicks );
            set => VisitDateTicks = value.Ticks;
        }

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
