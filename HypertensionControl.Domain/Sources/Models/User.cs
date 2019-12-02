using System;

namespace HypertensionControl.Domain.Models
{
    public class User
    {
        #region Auto-properties

        public Guid Id { get; private set; }

        public string Login { get; private set; }
        public string Role { get; private set; }

        public string Name { get; private set; }
        public string MiddleName { get; private set; }
        public string Surname { get; private set; }
        public string Position { get; private set; }
        public string ClinicName { get; private set; }
        public string ClinicAddress { get; private set; }
        #endregion
    }
}
