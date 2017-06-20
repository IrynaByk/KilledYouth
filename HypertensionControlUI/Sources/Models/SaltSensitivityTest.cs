using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HypertensionControlUI.Models
{
    [ComplexType]
    public class SaltSensitivityTest
    {
        public double? SaltSensitivity { get; set; }

        [NotMapped]
        public DateTime? TestDate
        {
            get => TestDateTicks != null ? new DateTime((long) TestDateTicks) : (DateTime?)null;
            set => TestDateTicks = value?.Ticks;
        }

        public long? TestDateTicks { get; set; }
    }
}