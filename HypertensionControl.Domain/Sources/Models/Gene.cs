namespace HypertensionControl.Domain.Models
{
    public class Gene
    {
        #region Auto-properties

        public string Name { get; set; }
        public int Value { get; set; }

        #endregion


        #region Initialization

        public Gene( string name, int value )
        {
            Name = name;
            Value = value;
        }

        #endregion
    }
}
