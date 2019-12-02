using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using HypertensionControl.Domain.Interfaces;
using HypertensionControl.Domain.Models.Values;
using HypertensionControl.Persistence.Entities;

namespace HypertensionControl.Persistence.Services
{
    public class SqliteDbContext : DbContext
    {
        #region Fields

        private readonly IResourceProvider _resourceProvider;

        #endregion


        #region Auto-properties

        public DbSet<ClassificationModelEntity> ClassificationModels { get; set; }
        public DbSet<PatientEntity> Patients { get; set; }
        public DbSet<UserEntity> Users { get; set; }

        #endregion


        #region Events and invocation

        protected override void OnModelCreating( DbModelBuilder modelBuilder )
        {
            Database.SetInitializer( new SqlDbInitializer( modelBuilder, _resourceProvider ) );
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<PatientEntity>()
                        .HasMany( p => p.VisitHistory )
                        .WithRequired( v => v.Patient )
                        .HasForeignKey( v => v.PatientId );
        }

        #endregion


        #region Initialization

        public SqliteDbContext( string connectionString, IResourceProvider resourceProvider ) : base( connectionString )
        {
            _resourceProvider = resourceProvider;
        }

        #endregion
    }
}
