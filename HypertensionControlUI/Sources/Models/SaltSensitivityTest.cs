using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HypertensionControlUI.Models
{
    [ComplexType]
    public class SaltSensitivityTest
    {
        public double? SaltSensitivity { get; set; }
        public DateTime? TestDate { get; set; }
    }
}