using HypertensionControlUI.Collections;

namespace HypertensionControlUI.Models.Runtime
{
    public class RuntimeModelProperty
    {
        #region Auto-properties

        public string Name { get; set; }
        public IScaler Scaler { get; set; }
        public double ModelCoefficient { get; set; }

        #endregion
    }
}