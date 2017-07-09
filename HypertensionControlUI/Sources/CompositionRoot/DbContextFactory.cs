using System;
using HypertensionControl.Persistence.Interfaces;
using HypertensionControlUI.Interfaces;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace HypertensionControlUI.CompositionRoot
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        #region Fields

        private readonly Container _container;

        #endregion


        #region Initialization

        public UnitOfWorkFactory( Container container )
        {
            _container = container;
        }

        #endregion


        #region Public methods

        public IUnitOfWork CreateUnitOfWork() => _container.GetInstance<IUnitOfWork>();

        #endregion
    }

    public class ScopedUnitOfWorkDecorator : IUnitOfWork
    {
        #region Fields

        private readonly IUnitOfWork _delegatee;
        private readonly Scope _scope;

        #endregion


        #region Properties

        public IUsersRepository UsersRepository => _delegatee.UsersRepository;

        public IPatientsRepository PatientsRepository => _delegatee.PatientsRepository;

        public IClinicsRepository ClinicsRepository => _delegatee.ClinicsRepository;

        public IClassificationModelsRepository ClassificationModelsRepository => _delegatee.ClassificationModelsRepository;

        #endregion


        #region Initialization

        public ScopedUnitOfWorkDecorator( Container container, Func<IUnitOfWork> decorateeFactory )
        {
            _scope = AsyncScopedLifestyle.BeginScope( container );
            _delegatee = decorateeFactory();
        }

        #endregion


        #region Public methods

        public void Dispose()
        {
            _delegatee.Dispose();
            _scope.Dispose();
        }

        public void SaveChanges() => _delegatee.SaveChanges();

        #endregion
    }
}
