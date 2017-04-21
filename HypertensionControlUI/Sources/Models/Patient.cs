using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace HypertensionControlUI.Models
{
    public class Patient
    {
        #region Initialization

        public Patient()
        {
            PatientVisitDataHistory = new List<PatientVisitData>();
            Genes = new List<Gene>();
            Medicine = new List<Medicine>();
        }

        #endregion

        #region Auto-properties

        //        [DatabaseGenerated( DatabaseGeneratedOption.Identity )]
        public int Id { get; set; }
        public string Name { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }

        [NotMapped]
        public DateTime BirthDate
        {
            get { return new DateTime(BirthDateTicks); }
            set { BirthDateTicks = value.Ticks; }
        }

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
        public int Age
        {
            get { return (DateTime.Now - BirthDate).Days/365; /* TODO: review  */ }
        }

        [NotMapped]
        public int? AGT_AGTR2
        {
            get
            {
                int? agt = null;
                int? agtr2 = null;
                foreach (var gene in Genes)
                {
                    if (gene.Name.Equals("AGT"))
                        agt = gene.Value;
                    if (gene.Name.Equals("AGTR2"))
                        agtr2 = gene.Value;
                }
                if (agt == null || agtr2 == null)
                    return null;
                if (agtr2 == 3 && agt >= 2)
                    return 1;
                return 0;
            }
        }

        [NotMapped]
        public HypertensionStage Stage
        {
            get { return PatientVisitDataHistory.OrderByDescending(d => d.VisitData).First().HypertensionStage; }
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
        public PatientVisitData()
        {
            VisitData = DateTime.Today;
            Smoking = new Smoking();
            DietaryHabits = new DietaryHabits {TesDate = DateTime.Today};
            BloodPressure = new BloodPressure();
            SaltSensitivity = new SaltSensitivityTest {TestDate = DateTime.Today};
        }

        public int Id { get; set; }
        public DateTime VisitData { get; set; }
        public AlcoholСonsumption AlcoholСonsumption { get; set; }
        public int StressPointsPSM_25 { get; set; }
        public int DepressionPointsCES_D { get; set; }
        public SaltSensitivityTest SaltSensitivity { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public double TemporaryBMI { get; set; }
        public double WaistCircumference { get; set; }
        public Smoking Smoking { get; set; }
        public DietaryHabits DietaryHabits { get; set; }
        public BloodPressure BloodPressure { get; set; }
        public PhysicalActivity PhysicalActivity { get; set; }
        public HypertensionStage HypertensionStage { get; set; }
        public virtual Patient Patient { get; set; }

        [NotMapped]
        public double BMI
        {
            get
            {
                if (Weight != 0 && Height != 0)
                    return Weight/Height/Height*10000;
                return TemporaryBMI;
            }
        }

        [NotMapped]
        public bool ObesityBMI
        {
            get { return BMI >= 30; }
        }

        [NotMapped]
        public bool ObesityWaistCircumference
        {
            get
            {
                if (Patient.Gender == 0)
                    return WaistCircumference > 88;

                return WaistCircumference > 102;
            }
        }
    }
}