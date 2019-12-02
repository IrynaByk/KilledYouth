using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HypertensionControl.Domain.Models.Values;

namespace HypertensionControl.Persistence.Entities
{
    /// <summary>
    ///     Describes a patient stored in a DB.
    /// </summary>
    [Table( "Patient" )]
    public sealed class PatientEntity
    {
        #region Auto-properties

        [Key]
        [Index]
        public string Id { get; set; }
        
        public string RegisteredBy { get; set; }

        public string Name { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string Address { get; set; }
        public string ClinicName { get; set; }
        public GenderType Gender { get; set; }
        public long BirthDateTicks { get; set; }
        public string BirthPlace { get; set; }
        public string Phone { get; set; }
        public string AccompanyingIllnesses { get; set; }
        public string Diagnosis { get; set; }
        public bool FemaleHeredity { get; set; }
        public bool MaleHeredity { get; set; }
        public HypertensionAncestralAnamnesis HypertensionAncestralAnamnesis { get; set; }
        public double HypertensionDuration { get; set; }
        public double? TreatmentDuration { get; set; }

        public string GenesSerialized { get; set; }
        public string MedicineSerialized { get; set; }

        public ICollection<PatientVisitEntity> VisitHistory { get; set; }

        #endregion
    }
}
