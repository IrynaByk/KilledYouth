using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HypertensionControl.Persistence.Entities
{
    [Table( "Clinic" )]
    public sealed class ClinicEntity
    {
        #region Auto-properties

        [Key]
        [Index]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }

        #endregion
    }
}
