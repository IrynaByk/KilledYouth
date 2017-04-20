using System.ComponentModel.DataAnnotations.Schema;

namespace HypertensionControlUI.Models
{
    public class Clinic
    {
//        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Address { get; set; }
        public string Name { get; set; }
        
    }
}