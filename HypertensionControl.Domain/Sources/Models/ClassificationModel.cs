using System.Collections.Generic;
using System.Linq;
using HypertensionControl.Domain.Models.Values;

namespace HypertensionControl.Domain.Models
{
    public class ClassificationModel
    {
        #region Auto-properties

        public string Name { get; }
        public string Description { get; }
        public ICollection<ClassificationModelProperty> Properties { get; }
        public double FreeCoefficient { get; }
        public double OptimalCutOff { get; }
        public RangedListScaler ResutClassificator { get; }

        #endregion


        #region Initialization

        public ClassificationModel( string name,
                                    string description,
                                    IList<double> limitPoints,
                                    ICollection<ClassificationModelProperty> properties,
                                    double freeCoefficient,
                                    double optimalCutOff )
        {
            Name = name;
            Description = description;

            Properties = properties;

            FreeCoefficient = freeCoefficient;
            OptimalCutOff = optimalCutOff;

            ResutClassificator = new RangedListScaler( limitPoints.OrderBy( p => p ).Select( ( p, i ) => new Range( p, i + 1 ) ) );
        }

        #endregion
    }
}
