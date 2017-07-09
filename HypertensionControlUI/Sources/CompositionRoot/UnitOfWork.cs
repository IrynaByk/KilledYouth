using System;
using HypertensionControl.Persistence.Interfaces;
using HypertensionControl.Persistence.Services;
using HypertensionControlUI.Interfaces;

namespace HypertensionControlUI.CompositionRoot
{
    public sealed class DbContextUnitOfWork : IUnitOfWork
    {
        #region Fields

        private readonly SqliteDbContext _dbContext;

        #endregion


        #region Auto-properties

        public IUsersRepository UsersRepository { get; }

        public IPatientsRepository PatientsRepository { get; }

        public IClinicsRepository ClinicsRepository { get; }

        public IClassificationModelsRepository ClassificationModelsRepository { get; }

        #endregion


        #region Initialization

        public DbContextUnitOfWork( SqliteDbContext dbContext,
                                    IUsersRepository usersRepository,
                                    IPatientsRepository patientsRepository,
                                    IClinicsRepository clinicsRepository,
                                    IClassificationModelsRepository classificationModelsRepository )
        {
            _dbContext = dbContext;
            UsersRepository = usersRepository;
            PatientsRepository = patientsRepository;
            ClinicsRepository = clinicsRepository;
            ClassificationModelsRepository = classificationModelsRepository;
        }

        #endregion


        #region Public methods

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        #endregion
    }
}
