using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.IO;
using System.Linq;
using HypertensionControlUI.Models;
using HypertensionControlUI.Utils;

namespace HypertensionControlUI.Services
{
    public interface IDbContext : IDisposable
    {
        #region Properties

        DbSet<User> Users { get; set; }
        DbSet<Clinic> Clinics { get; set; }
        DbSet<Patient> Patients { get; set; }
        DbSet<ClassificationModel> ClassificationModels { get; set; }

        #endregion


        #region Public methods

        int SaveChanges();
        void Attach<TEntity>( TEntity entity, bool modified = true ) where TEntity : class;
        void AttachNew<TEntity>( TEntity entity ) where TEntity : class;

        #endregion
    }

    internal class FakeDbContext : IDbContext
    {
        #region Auto-properties

        public DbSet<User> Users { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<ClassificationModel> ClassificationModels { get; set; }

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

    public class SqlDbContext : DbContext, IDbContext
    {
        #region Auto-properties

        public DbSet<User> Users { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<ClassificationModel> ClassificationModels { get; set; }

        #endregion


        #region Events and invocation

        protected override void OnModelCreating( DbModelBuilder modelBuilder )
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<ClassificationModel>()
                        .HasMany( model => model.Properties )
                        .WithRequired()
                        .HasForeignKey( p => p.ModelId )
                        .WillCascadeOnDelete();

            modelBuilder.Entity<ModelProperty>().HasMany( p => p.Entries ).WithRequired().WillCascadeOnDelete();
            modelBuilder.Entity<Patient>().HasMany( p => p.Genes ).WithRequired().HasForeignKey( g => g.PatientId );
            modelBuilder.Entity<Patient>().HasMany( p => p.Medicine ).WithRequired();
            modelBuilder.Entity<PatientVisitData>().HasRequired( pvd => pvd.Patient ).WithMany( p => p.PatientVisitDataHistory );
            modelBuilder.Entity<ClassificationModel>().HasMany( p => p.LimitPoints ).WithRequired();
        }

        #endregion


        #region Initialization

        public SqlDbContext( string connectionString ) : base( connectionString )
        {
            Database.CreateIfNotExists();
        }


        public SqlDbContext() : base( "Hypertension" )
        {
        }

        #endregion


        #region Public methods

        public void Attach<TEntity>( TEntity entity, bool modified = true ) where TEntity : class
        {
            Entry( entity ).State = modified ? EntityState.Modified : EntityState.Unchanged;
        }

        public void AttachNew<TEntity>( TEntity entity ) where TEntity : class
        {
            Entry( entity ).State = EntityState.Added;
        }

        #endregion
    }
}
