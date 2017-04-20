using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HypertensionControlUI.Models
{
    public class ModelProperty
    {
        #region Auto-properties

        [Key, Column( Order = 0 )]
        public int ModelId { get; set; }

        [Key, Column( Order = 1 )]
        public string Name { get; set; }

        public virtual ICollection<ModelScaleEntry> Entries { get; set; }
        public double ModelCoefficient { get; set; }

        #endregion
    }
}
