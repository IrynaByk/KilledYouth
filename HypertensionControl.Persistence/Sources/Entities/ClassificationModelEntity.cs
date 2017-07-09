using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HypertensionControl.Persistence.Entities
{
    [Table("ClassificationModel")]
    public sealed class ClassificationModelEntity
    {
        #region Auto-properties

        [Key, Index]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string LimitPointsSerialized { get; set; }
        public double FreeCoefficient { get; set; }
        public string PropertiesSerialized { get; set; }
        public double OptimalCutOff { get; set; }

        #endregion
    }
}
