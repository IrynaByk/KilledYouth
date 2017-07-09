namespace HypertensionControl.Domain.Models.Values
{
    public class Smoking
    {
        #region Auto-properties

        public SmokingType Type { get; set; }
        public int CigarettesPerDay { get; set; }
        public double DurationInYears { get; set; }

        #endregion
    }
}
