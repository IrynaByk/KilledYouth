using System.Windows;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using HypertensionControl.Persistence.Interfaces;
using HypertensionControl.Persistence.Services;
using HypertensionControlUI.CompositionRoot;
using HypertensionControlUI.Interfaces;
using HypertensionControlUI.Services;
using HypertensionControlUI.ViewModels;
using HypertensionControlUI.Views;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace HypertensionControlUI
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Events and invocation

        protected override void OnStartup( StartupEventArgs e )
        {
            base.OnStartup( e );
            var container = Bootstrap();

            Mapper.Initialize( expression => expression.CreateMissingTypeMaps = true );

            ComposeObjects( container );
        }

        #endregion


        #region Non-public methods

        /// <summary>
        ///     Creates and configures the application window.
        /// </summary>
        /// <param name="container">The IoC container.</param>
        private void ComposeObjects( Container container )
        {
            Current.MainWindow = container.GetInstance<MainWindow>();
            Current.MainWindow.Show();
            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            var provider = (ViewProvider) container.GetInstance<IViewProvider>();
            provider.NavigateToPage<MainViewModel>();
        }

        /// <summary>
        ///     Configures the IoC container.
        /// </summary>
        /// <returns>The configured and verified IoC container.</returns>
        private static Container Bootstrap()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            //  Provides resources embedded into the application
            var resourceProvider = new ResourceProvider();
            container.RegisterSingleton( resourceProvider );

            //  Configure and register AutoMapper engine
            container.RegisterSingleton( () => new MapperConfiguration( expr =>
            {
                expr.AddCollectionMappers();
                expr.AddProfile( new SqliteDbMappingProfile() );
            } ).CreateMapper() );

            //  Configure and register DbContext as a Scoped instance
            container.Register( () => new SqliteDbContext( "SQLiteDB", resourceProvider ), Lifestyle.Scoped );

            //  Register the UnitOfWork instance and factory
            container.Register<IUnitOfWork, DbContextUnitOfWork>();
            container.RegisterDecorator<IUnitOfWork, ScopedUnitOfWorkDecorator>();
            container.RegisterSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();

            //  Register the persistence repositories
            container.Register<IUsersRepository, UsersRepository>(Lifestyle.Scoped);
            container.Register<IPatientsRepository, PatientsRepository>(Lifestyle.Scoped);
            container.Register<IClassificationModelsRepository, ClassificationModelsRepository>(Lifestyle.Scoped);

            //  Register the Identity service
            container.RegisterSingleton<IdentityService>();

            container.RegisterSingleton<IViewProvider, ViewProvider>();

            container.RegisterSingleton<MainWindowViewModel>();
            container.RegisterSingleton<MainWindow>();
            container.RegisterSingleton( () => container.GetInstance<MainWindow>().MainWindowFrame );

            container.Register( typeof(PageViewBase<>), new[] { typeof(App).Assembly } );
            container.RegisterSingleton<ISettingsProvider, SettingsProvider>();

            //  Register View-Models
            container.Register<LoginViewModel>();
            container.Register<MainViewModel>();
            container.Register<PatientsViewModel>();
            container.Register<UserViewModel>();
            container.Register<AddPatientViewModel>();
            container.Register<IndividualPatientCardViewModel>();
            container.Register<ClassificationTunningViewModel>();

            container.Register<PatientClassificatorFactory>();

            //  Check the IoC registrations graph consistency
//            container.Verify();

            return container;
        }

        #endregion
    }
}
