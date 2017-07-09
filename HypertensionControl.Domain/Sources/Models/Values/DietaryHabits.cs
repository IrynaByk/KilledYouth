using System;

namespace HypertensionControl.Domain.Models.Values
{
    public class DietaryHabits
    {
        #region Auto-properties

        public DateTime TestDate { get; set; }

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

        #endregion
    }
}
