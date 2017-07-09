using HypertensionControl.Domain.Interfaces;

namespace HypertensionControlUI.Interfaces
{
    public interface IUnitOfWorkFactory
    {
        #region Public methods

        IUnitOfWork CreateUnitOfWork();

        #endregion
    }
}
