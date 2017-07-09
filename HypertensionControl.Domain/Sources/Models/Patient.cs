using System;
using System.Collections.Generic;
using System.Linq;
using HypertensionControl.Domain.Models.Values;

namespace HypertensionControl.Domain.Models
{
    /// <summary>
    ///     Describes a patient stored in a DB.
    /// </summary>
    public class Patient
    {
        #region Auto-properties

        /// <summary>
        ///     A unique patient ID.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        ///     ID of the patient's clinic.
        /// </summary>
        public Guid ClinicId { get; set; }

        /// <summary>
        ///     Name of the doctor who has registered the patient.
        /// </summary>
        public string RegisteredBy { get; private set; }

        /// <summary>
        ///     First name of the patient.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Middle name of the patient.
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        ///     Surname of the patient.
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        ///     Patient address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        ///     Patient gender.
        /// </summary>
        public GenderType Gender { get; set; }

        /// <summary>
        ///     Patient birth date.
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        ///     Patient birthplace.
        /// </summary>
        public string BirthPlace { get; set; }

        /// <summary>
        ///     Patient nationality.
        /// </summary>
        public string Nationality { get; set; }

        /// <summary>
        ///     Patient phone number.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        ///     Patient accompanying ilnesses listed as a string.
        /// </summary>
        public string AccompanyingIllnesses { get; set; }

        /// <summary>
        ///     Patient diagnosis.
        /// </summary>
        public string Diagnosis { get; set; }

        /// <summary>
        ///     Patient heredity from female parent.
        /// </summary>
        public bool FemaleHeredity { get; set; }

        /// <summary>
        ///     Patient heredity from male parent.
        /// </summary>
        public bool MaleHeredity { get; set; }

        public HypertensionAncestralAnamnesis HypertensionAncestralAnamnesis { get; set; }

        public double HypertensionDuration { get; set; }

        public double? TreatmentDuration { get; set; }

        public ICollection<Medicine> Medicine { get; set; }

        public IDictionary<string, Gene> Genes { get; private set; }
        public ICollection<PatientVisit> VisitHistory { get; private set; }

        #endregion


        #region Properties

        public PatientVisit LastVisit => VisitHistory.OrderByDescending( pvd => pvd.VisitDate ).First();

        public HypertensionStage? HypertensionStage => LastVisit.HypertensionStage;

        public int Age
        {
            get => (DateTime.Now - BirthDate).Days / 365;
            set => BirthDate = DateTime.Now - TimeSpan.FromDays( value * 365 );
        }

        public int? AgtAgtr2
        {
            get
            {
                Genes.TryGetValue( GenesNames.Agt, out var agtGene );
                var agt = agtGene?.Value;

                Genes.TryGetValue(GenesNames.Agtr2, out var agtr2Gene );
                var agtr2 = agtr2Gene?.Value;

                if ( agt == null || agtr2 == null )
                {
                    return null;
                }
                if ( agtr2 == 3 && agt >= 2 )
                {
                    return 1;
                }
                return 0;
            }
        }

        #endregion


        #region Initialization

        private Patient()
        {
        }

        #endregion


        #region Public methods

        public static Patient CreateNew( string registeredBy )
        {
            var patient = new Patient
            {
                Id = Guid.NewGuid(),
                RegisteredBy = registeredBy,
                Medicine = new List<Medicine>(),
                Genes = new Dictionary<string, Gene>(),
                VisitHistory = new List<PatientVisit>()
            };

            return patient;
        }

        public PatientVisit AddVisit()
        {
            var visit = PatientVisit.CreateTodayVisit( this );
            VisitHistory.Add( visit );
            return visit;
        }

        public void SetGeneValue( string geneName, GeneValue value )
        {
            if ( value == GeneValue.None )
            {
                Genes.Remove( geneName );
            }
            else if ( Genes.TryGetValue( geneName, out var gene ) )
            {
                gene.Value = Convert.ToInt32( value );
            }
            else
            {
                Genes.Add( geneName, new Gene( geneName, (int) value ) );
            }
        }

        public GeneValue GetGeneValue( string geneName )
        {
            return Genes.TryGetValue( geneName, out var value ) ? (GeneValue) value.Value : GeneValue.None;
        }

        #endregion
    }
}
