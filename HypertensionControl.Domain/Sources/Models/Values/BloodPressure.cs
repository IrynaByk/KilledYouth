namespace HypertensionControl.Domain.Models.Values
{
    /// <summary>
    ///     Describes blood pressure characteristics.
    /// </summary>
    public class BloodPressure
    {
        #region Auto-properties

        public int RightShoulderSbp { get; set; }
        public int RightShoulderDbp { get; set; }
        public int LeftShoulderSbp { get; set; }
        public int LeftShoulderDbp { get; set; }
        public int HeartRate { get; set; }

        #endregion
    }
}
