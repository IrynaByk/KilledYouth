using System;
using System.Data.Entity;
using HypertensionControlUI.Models;

namespace HypertensionControlUI.Services
{
    /// <summary>
    ///     Describes the data context used to access the DB data.
    /// </summary>
    public interface IDbContext : IDisposable
    {
        #region Properties

        DbSet<ClassificationModel> ClassificationModels { get; set; }
        DbSet<Clinic> Clinics { get; set; }
        DbSet<Patient> Patients { get; set; }
        DbSet<User> Users { get; set; }

        #endregion


        #region Public methods

        int SaveChanges();
        void Attach<TEntity>( TEntity entity, bool modified = true ) where TEntity : class;
        void AttachNew<TEntity>( TEntity entity ) where TEntity : class;

        #endregion
    }
}
