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

        public int CigarettesPerDay { get; set; }
        public double DurationInYears { get; set; }

        public SmokingType Type { get; set; }

        #endregion
    }
}
