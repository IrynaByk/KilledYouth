using System;

namespace HypertensionControl.Domain.Models
{
    public class Clinic
    {
        #region Auto-properties

        public Guid Id { get; }
        public string Name { get; }
        public string Address { get; }

        #endregion


        #region Initialization

        public Clinic( Guid id, string name, string address )
        {
            Id = id;
            Name = name;
            Address = address;
        }

        #endregion


        #region Public methods

        public static Clinic CreateNew( string name, string address )
        {
            return new Clinic( Guid.NewGuid(), name, address );
        }

        #endregion
    }
}
