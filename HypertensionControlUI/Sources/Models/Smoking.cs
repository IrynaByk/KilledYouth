using System.ComponentModel.DataAnnotations.Schema;

namespace HypertensionControlUI.Models
{
    public enum SmokingType
    {
        Never,
        InPast,
        Now
    }

    [ComplexType]
    public class Smoking
    {
        #region Auto-properties

        public SmokingType Type { get; set; }
        public double DurationInYears { get; set; }
        public int CigarettesPerDay { get; set; }

        #endregion
    }
}
