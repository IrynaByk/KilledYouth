using System.Collections.Generic;
using System.Linq;
using HypertensionControlUI.Collections;

namespace HypertensionControlUI.Models.Runtime
{
    public class RuntimeClassificationModel
    {
        #region Auto-properties

        public ICollection<RuntimeModelProperty> Properties { get; set; }
        public double FreeCoefficient { get; set; }
        public RangedListScaler ResutClassificator { get; set; }
        public double OptimalCutOff { get; set; }

        #endregion


        #region Initialization

        public RuntimeClassificationModel( ClassificationModel model )
        {
            FreeCoefficient = model.FreeCoefficient;
            var rangeEntries = model.LimitPoints
                                    .OrderBy( p => p )
                                    .Select( ( p, i ) => new RangeEntry( p.Point, i + 1 ) );

            ResutClassificator = new RangedListScaler( rangeEntries );

            Properties = model.Properties
                              .Select( p => new RuntimeModelProperty
                              {
                                  Name = p.Name,
                                  Scaler = p.ScaleEntries.Any()
                                      ? new RangedListScaler( p.ScaleEntries.Select( e => new RangeEntry( e.LowerBound, e.Value ) ) )
                                      : (IScaler) new IdentityScaler(),
                                  Coefficient = p.Coefficient
                              } )
                              .ToList();


        }

        #endregion
    }
}
