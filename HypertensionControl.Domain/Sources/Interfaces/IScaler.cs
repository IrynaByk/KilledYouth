namespace HypertensionControl.Domain.Interfaces
{
    public interface IScaler
    {
        #region Properties

        double this[ double position ] { get; }

        #endregion
    }
}