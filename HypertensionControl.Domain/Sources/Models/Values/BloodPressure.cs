namespace HypertensionControl.Domain.Models.Values
{
    /// <summary>
    ///     Describes blood pressure characteristics.
    /// </summary>
    public class BloodPressure
    {
        #region Auto-properties

        public double RightShoulderSbp { get; set; }
        public double RightShoulderDbp { get; set; }
        public double LeftShoulderSbp { get; set; }
        public double LeftShoulderDbp { get; set; }
        public double HeartRate { get; set; }

        #endregion
    }
}
