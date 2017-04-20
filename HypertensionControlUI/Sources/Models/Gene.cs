using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HypertensionControlUI.Models
{
    public class Gene
    {
        [Key, Column(Order = 0)]
        public int PatientId { get; set; }
        [Key, Column(Order = 1)]
        public string Name { get; set; }
        public int Value { get; set; }
    }
}