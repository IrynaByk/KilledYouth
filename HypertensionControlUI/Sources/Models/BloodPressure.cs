using System.ComponentModel.DataAnnotations.Schema;

namespace HypertensionControlUI.Models
{
    [ComplexType]
    public class BloodPressure
    {
        #region Auto-properties

        public double RightShoulderSBP { get; set; }
        public double RightShoulderDBP { get; set; }
        public double LeftShoulderSBP { get; set; }
        public double LeftShoulderDBP { get; set; }
        public double HeartRate { get; set; }

        #endregion
    }
}
