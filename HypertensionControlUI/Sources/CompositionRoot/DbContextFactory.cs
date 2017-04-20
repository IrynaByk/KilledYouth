using HypertensionControlUI.Services;
using SimpleInjector;

namespace HypertensionControlUI.CompositionRoot
{
    public class DbContextFactory
    {
        #region Fields

        private readonly Container _container;

        #endregion


        #region Initialization

        public DbContextFactory( Container container )
        {
            _container = container;
        }

        #endregion


        #region Public methods

        public IDbContext GetDbContext()
        {
            return _container.GetInstance<IDbContext>();
        }

        #endregion
    }
}
