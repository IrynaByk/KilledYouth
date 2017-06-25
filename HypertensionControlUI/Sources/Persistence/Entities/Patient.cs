using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using HypertensionControlUI.Models;

namespace HypertensionControlUI.Persistence.Entities
{
    /// <summary>
    ///     Describes a patient stored in a DB.
    /// </summary>
    public class PatientEntity
    {
        #region Auto-properties

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

        public int Id { get; set; }

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
        public DateTime BirthDate
        {
            get => new DateTime( BirthDateTicks );
            set => BirthDateTicks = value.Ticks;
        }

        [NotMapped]
        public HypertensionStage? HypertensionStage => LastVisitData.HypertensionStage;

        [NotMapped]
        public PatientVisitData LastVisitData => PatientVisitHistory.OrderByDescending( pvd => pvd.VisitDate ).First();

        #endregion


        #region Initialization

        public PatientEntity()
        {
            PatientVisitHistory = new List<PatientVisitData>();
            Medicine = new List<Medicine>();
        }

        #endregion
    }
}
