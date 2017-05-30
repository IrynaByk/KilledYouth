using System.Collections.Generic;

namespace HypertensionControlUI.Models
{
    public class ClassificationModel
    {
        #region Auto-properties

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<LimitPoint> LimitPoints { get; set; }
        public double FreeCoefficient { get; set; }
        public virtual ICollection<ModelProperty> Properties { get; set; }
        public double OptimalCutOff { get; set; }

        #endregion
    }
}
