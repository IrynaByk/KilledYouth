using System.Data.Entity;
using HypertensionControlUI.Models;

namespace HypertensionControlUI.Services
{
    internal class FakeDbContext : IDbContext
    {
        #region Auto-properties

        public DbSet<ClassificationModel> ClassificationModels { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Patient> Patients { get; set; }

        public DbSet<User> Users { get; set; }

        #endregion


        #region Public methods

        public void Dispose()
        {
        }

        public int SaveChanges()
        {
            return 0;
        }

        public void Attach<TEntity>( TEntity entity, bool modified = true ) where TEntity : class
        {
        }

        public void AttachNew<TEntity>( TEntity entity ) where TEntity : class
        {
        }

        #endregion
    }
}