namespace HypertensionControl.Domain.Models.Values
{
    /// <summary>
    ///     Describes a medicine and it's dose.
    /// </summary>
    public class Medicine
    {
        #region Auto-properties

        public string Name { get; }
        public string Dose { get; }

        #endregion


        #region Initialization

        public Medicine( string name, string dose )
        {
            Name = name;
            Dose = dose;
        }

        public Medicine()
        {
        }

        #endregion
    }
}
