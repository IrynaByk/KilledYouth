using System.Collections.Generic;
using HypertensionControl.Domain.Interfaces;
using HypertensionControl.Domain.Models.Values;

namespace HypertensionControl.Domain.Models
{
    /// <summary>
    ///     A classification model property.
    /// </summary>
    public class ClassificationModelProperty
    {
        #region Auto-properties

        /// <summary>
        ///     Property name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Property coefficient.
        /// </summary>
        public double Coefficient { get; }

        /// <summary>
        ///     Optional property scaler.
        /// </summary>
        public IScaler Scaler { get; }

        #endregion


        #region Initialization

        public ClassificationModelProperty( string name, double coefficient, ICollection<Range> scaleRanges = null )
        {
            Name = name;
            Coefficient = coefficient;

            //  Create scaler fopr the provided scale ranges
            if ( scaleRanges?.Count > 0 )
            {
                Scaler = new RangedListScaler( scaleRanges );
            }
            else
            {
                Scaler = new IdentityScaler();
            }
        }

        #endregion


        #region Public methods

        public override string ToString()
        {
            return $"{Name} * {Scaler} * {Coefficient}";
        }

        #endregion
    }
}
