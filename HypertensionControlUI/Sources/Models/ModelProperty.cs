using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HypertensionControlUI.Models
{
    /// <summary>
    ///     A single model property.
    /// </summary>
    public class ModelProperty
    {
        #region Auto-properties

        /// <summary>
        ///     The property coefficient.
        /// </summary>
        public double Coefficient { get; set; }

        /// <summary>
        ///     Reference to the model.
        /// </summary>
        [Key]
        [Column( Order = 0 )]
        public int ModelId { get; set; }

        /// <summary>
        ///     Name of the property.
        /// </summary>
        [Key]
        [Column( Order = 1 )]
        public string Name { get; set; }

        /// <summary>
        ///     A property scaling rules.
        /// </summary>
        public virtual ICollection<ModelScaleEntry> ScaleEntries { get; set; }

        #endregion
    }
}
