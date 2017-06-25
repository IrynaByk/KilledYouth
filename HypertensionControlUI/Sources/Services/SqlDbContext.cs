using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using HypertensionControlUI.Models;

namespace HypertensionControlUI.Services
{
    public class SqlDbContext : DbContext, IDbContext
    {
        #region Fields

        private readonly ResourceProvider _resourceProvider;

        #endregion


        #region Auto-properties

        public DbSet<ClassificationModel> ClassificationModels { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Patient> Patients { get; set; }

        public DbSet<User> Users { get; set; }

        #endregion


        #region Events and invocation

        protected override void OnModelCreating( DbModelBuilder modelBuilder )
        {
            Database.SetInitializer( new SqlDbInitializer( modelBuilder, _resourceProvider ) );

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<ClassificationModel>()
                        .HasMany( model => model.Properties )
                        .WithRequired()
                        .HasForeignKey( p => p.ModelId )
                        .WillCascadeOnDelete();

            modelBuilder.Entity<ModelProperty>().HasMany( p => p.ScaleEntries ).WithRequired().WillCascadeOnDelete();
            modelBuilder.Entity<Patient>().HasMany( p => p.Genes ).WithRequired().HasForeignKey( g => g.PatientId );
            modelBuilder.Entity<Patient>().HasMany( p => p.Medicine ).WithRequired();
            modelBuilder.Entity<PatientVisitData>().HasRequired( pvd => pvd.Patient ).WithMany( p => p.PatientVisitHistory );
            modelBuilder.Entity<ClassificationModel>().HasMany( p => p.LimitPoints ).WithRequired();
        }

        #endregion


        #region Initialization

        public SqlDbContext( string connectionString, ResourceProvider resourceProvider ) : base( connectionString )
        {
            _resourceProvider = resourceProvider;
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
