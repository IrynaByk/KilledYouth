using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HypertensionControl.Persistence.Entities
{
    [Table( "User" )]
    public sealed class UserEntity
    {
        #region Auto-properties

        [Key]
        [Index]
        public string Id { get; set; }

        [Index]
        public string Login { get; set; }

        [Index]
        public string PasswordHash { get; set; }

        public string Name { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }

        public string ClinicName { get; set; }
        public string ClinicAddress { get; set; }

        public string Position { get; set; }
        public string Role { get; set; }

        #endregion
    }
}
