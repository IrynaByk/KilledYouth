using HypertensionControl.Domain.Interfaces;

namespace HypertensionControl.Domain.Models.Values
{
    /// <summary>
    ///     Scaler that maps each value into itself.
    /// </summary>
    internal class IdentityScaler : IScaler
    {
        #region Properties

        public double this[ double position ] => position;

        #endregion


        #region Public methods

        public override string ToString()
        {
            return "1";
        }

        #endregion
    }
}
