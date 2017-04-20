using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HypertensionControlUI.Models
{
    public enum Role
    {
        User,
        Admin
    }
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public Clinic Job { get; set; }
        public string Position { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public Role Role { get; set; }
    
    }

}