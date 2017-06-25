using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using HypertensionControlUI.CompositionRoot;
using HypertensionControlUI.Models;
using HypertensionControlUI.Services;
using HypertensionControlUI.Utils;

namespace HypertensionControlUI.ViewModels
{
    public class PatientsViewModel : PageViewModelBase
    {
        #region Fields

        private readonly DbContextFactory _dbContextFactory;
        private readonly IdentityService _identityService;
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly IViewProvider _viewProvider;
        private string _patientFilter = "";
        private List<Patient> _patients;
        private Patient _selectedPatient;

        #endregion


        #region Auto-properties

        public ICommand AddPatientCommand { get; }

        public AsyncDelegateCommand<Patient> AddPatientVisitCommand { get; set; }
        public AsyncDelegateCommand<Patient> EditPatientCommand { get; set; }
        public ICollectionView PatientsView { get; private set; }
        public AsyncDelegateCommand<Patient> ShowPatientCommand { get; set; }
        public AsyncDelegateCommand ShowPatientStatistics { get; set; }

        #endregion


        #region Properties

        public string PatientFilter
        {
            get => _patientFilter;
            set
            {
                _patientFilter = value;
                PatientsView.Refresh();
            }
        }

        public List<Patient> Patients
        {
            get => _patients;
            set
            {
                if ( _patients == value )
                    return;
                _patients = value;
                PatientsView = CollectionViewSource.GetDefaultView( _patients );
                PatientsView.Filter = PatientsFilter;
                PatientsView.SortDescriptions.Add( new SortDescription( nameof(Patient.Surname), ListSortDirection.Ascending ) );
                PatientsView.SortDescriptions.Add( new SortDescription( nameof(Patient.Name), ListSortDirection.Ascending ) );
                PatientsView.SortDescriptions.Add( new SortDescription( nameof(Patient.MiddleName), ListSortDirection.Ascending ) );
            }
        }

        public Patient SelectedPatient
        {
            get => _selectedPatient;
            set
            {
                if ( Equals( value, _selectedPatient ) )
                    return;
                _selectedPatient = value;
                OnPropertyChanged();
            }
        }

        #endregion


        #region Initialization

        public PatientsViewModel( IViewProvider viewProvider,
                                  DbContextFactory dbContextFactory,
                                  IdentityService identityService,
                                  MainWindowViewModel mainWindowViewModel )
        {
            _viewProvider = viewProvider;
            _dbContextFactory = dbContextFactory;
            _identityService = identityService;
            _mainWindowViewModel = mainWindowViewModel;
            using ( var db = _dbContextFactory.GetDbContext() )
            {
                Patients = db.Patients
                             .Include( p => p.PatientVisitDataHistory )
                             .Include( p => p.Clinic )
                             .Include( p => p.Medicine )
                             .Include( p => p.Genes )
                             .ToList();
            }

            AddPatientCommand = new AsyncDelegateCommand( o => _viewProvider.NavigateToPage<AddPatientViewModel>( m =>
            {
                m.Patient = new Patient();
                m.Patient.CreatedBy = _identityService.CurrentUser.Login;
                m.ActualPatientVisitData = new PatientVisitData { Patient = m.Patient };
            } ) );

            AddPatientVisitCommand = new AsyncDelegateCommand<Patient>(patient => _viewProvider.NavigateToPage<AddPatientViewModel>( m =>
            {
                _mainWindowViewModel.Patient = patient ?? SelectedPatient;
                m.Patient = _mainWindowViewModel.Patient;
                m.ActualPatientVisitData = new PatientVisitData { Patient = m.Patient };
            } ) );

            EditPatientCommand = new AsyncDelegateCommand<Patient>( patient => _viewProvider.NavigateToPage<AddPatientViewModel>( m =>
            {
                _mainWindowViewModel.Patient = patient ?? SelectedPatient;

                m.Patient = _mainWindowViewModel.Patient;
                m.ActualPatientVisitData = _mainWindowViewModel.Patient
                                                               .PatientVisitDataHistory
                                                               .OrderByDescending( pvd => pvd.VisitDate )
                                                               .First();
            } ) );

            ShowPatientCommand = new AsyncDelegateCommand<Patient>( patient => _viewProvider.NavigateToPage<IndividualPatientCardViewModel>( m =>
            {
                _mainWindowViewModel.Patient = patient ?? SelectedPatient;
                m.Patient = _mainWindowViewModel.Patient;
                m.PatientVisitData =
                    _mainWindowViewModel.Patient
                                        .PatientVisitDataHistory
                                        .OrderByDescending( pvd => pvd.VisitDate )
                                        .First();
            } ) );

            ShowPatientStatistics = new AsyncDelegateCommand( o => _viewProvider.NavigateToPage<PatientStatisticsViewModel>() );
        }

        #endregion


        #region Non-public methods

        private bool PatientsFilter( object o )
        {
            return o is Patient patient &&
                   (patient.CreatedBy == _identityService.CurrentUser.Login ||
                    _identityService.CurrentUser.Role == Role.Admin) &&
                   (patient.Name.IndexOf( PatientFilter, StringComparison.InvariantCultureIgnoreCase ) != -1 ||
                    patient.MiddleName.IndexOf( PatientFilter, StringComparison.InvariantCultureIgnoreCase ) != -1 ||
                    patient.Surname.IndexOf( PatientFilter, StringComparison.InvariantCultureIgnoreCase ) != -1);
        }

        #endregion
    }
}
