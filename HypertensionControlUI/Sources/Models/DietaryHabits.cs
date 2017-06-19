using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HypertensionControlUI.Models
{
    public enum DietaryFrequency
    {
        Never,
        OneTwo,
        ThreeFive,
        SixSeven
    }
    [ComplexType]
    public class DietaryHabits
    {
        public DietaryFrequency Potato { get; set; }
        public DietaryFrequency Rice { get; set; }
        public DietaryFrequency Poridge { get; set; }
        public DietaryFrequency Dairy { get; set; }
        public DietaryFrequency Fish { get; set; }
        public DietaryFrequency Meat { get; set; }
        public DietaryFrequency Sausage { get; set; }
        public DietaryFrequency FreshVegetables { get; set; }
        public DietaryFrequency CookedVegetables { get; set; }
        public DietaryFrequency Fruite { get; set; }
        public DietaryFrequency CannedFruite { get; set; }
        public DietaryFrequency Sweets { get; set; }
        public DietaryFrequency ColdDrinks { get; set; }
        public DietaryFrequency Eggs { get; set; }
        [NotMapped]
        public DateTime TesDate
        {
            get => new DateTime( TestDateTicks);
            set => TestDateTicks = value.Ticks;
        }

        public long TestDateTicks { get; set; }
    }
}