using System.Windows;
using AutoMapper;
using HypertensionControlUI.CompositionRoot;
using HypertensionControlUI.Services;
using HypertensionControlUI.ViewModels;
using HypertensionControlUI.Views;
using SimpleInjector;
using SimpleInjector.Diagnostics;

namespace HypertensionControlUI
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Events and invocation

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var container = Bootstrap();

            Mapper.Initialize(expression => expression.CreateMissingTypeMaps = true);

            ComposeObjects(container);
        }

        #endregion


        #region Non-public methods

        private void ComposeObjects(Container container)
        {
            Current.MainWindow = container.GetInstance<MainWindow>();
            Current.MainWindow.Show();
            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            var provider = (ViewProvider) container.GetInstance<IViewProvider>();
            provider.NavigateToPage<MainViewModel>();
        }

        private static Container Bootstrap()
        {
            var container = new Container();

            // container.Register<IDbContext, FakeDbContext>();
            container.Register<IDbContext>(() => new SqlDbContext("SQLiteDB"));

            //new SqlDbContext( container.GetInstance<ISettingsProvider>().ConnectionString ) );
            container.RegisterSingleton<DbContextFactory>();
            container.RegisterSingleton<IdentityService>();

            container.RegisterSingleton<IViewProvider, ViewProvider>();

            container.RegisterSingleton<MainWindowViewModel>();
            container.RegisterSingleton<MainWindow>();
            container.RegisterSingleton(() => container.GetInstance<MainWindow>().MainWindowFrame);

            //            container.Register( typeof (WindowViewBase<>), new[] { typeof (App).Assembly } );
            container.Register(typeof(PageViewBase<>), new[] {typeof(App).Assembly});
            container.RegisterSingleton<ISettingsProvider, SettingsProvider>();
            container.Register<LoginViewModel>();
            container.Register<MainViewModel>();
            container.Register<PatientsViewModel>();
            container.Register<UserViewModel>();
            container.Register<AddPatientViewModel>();
            container.Register<PatientClassificatorFactory>();
            container.Register<IndividualPatientCardViewModel>();
            container.Register<ClassificationTunningViewModel>();

            container.GetRegistration(typeof(IDbContext))
                     .Registration
                     .SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Just because");

//            container.Verify();
            return container;
        }

        #endregion
    }
}