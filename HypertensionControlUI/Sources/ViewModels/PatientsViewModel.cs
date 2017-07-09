using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using HypertensionControl.Domain.Models;
using HypertensionControlUI.Interfaces;
using HypertensionControlUI.Services;
using HypertensionControlUI.Utils;

namespace HypertensionControlUI.ViewModels
{
    public class PatientsViewModel : PageViewModelBase
    {
        #region Fields

        private readonly IdentityService _identityService;
        private readonly MainWindowViewModel _mainWindowViewModel;

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
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
                if ( Set( ref _patients, value ) )
                {
                    PatientsView = CollectionViewSource.GetDefaultView( _patients );
                    PatientsView.Filter = PatientsFilter;
                    PatientsView.SortDescriptions.Add( new SortDescription( nameof(Patient.Surname), ListSortDirection.Ascending ) );
                    PatientsView.SortDescriptions.Add( new SortDescription( nameof(Patient.Name), ListSortDirection.Ascending ) );
                    PatientsView.SortDescriptions.Add( new SortDescription( nameof(Patient.MiddleName), ListSortDirection.Ascending ) );
                }
            }
        }

        public Patient SelectedPatient
        {
            get => _selectedPatient;
            set => Set( ref _selectedPatient, value );
        }

        #endregion


        #region Commands

        private void ShowPatientStatisticsCommandHandler( object o )
        {
            _viewProvider.NavigateToPage<PatientStatisticsViewModel>();
        }

        private void ShowPatientCommandHandler( Patient patient )
        {
            _viewProvider.NavigateToPage<IndividualPatientCardViewModel>( m =>
            {
                _mainWindowViewModel.Patient = patient ?? SelectedPatient;
                m.Patient = _mainWindowViewModel.Patient;
                m.PatientVisit = _mainWindowViewModel.Patient.LastVisit;
            } );
        }

        private void EditPatientCommandHandler( Patient patient )
        {
            _viewProvider.NavigateToPage<AddPatientViewModel>( m =>
            {
                _mainWindowViewModel.Patient = patient ?? SelectedPatient;

                m.Patient = _mainWindowViewModel.Patient;
                m.ActualPatientVisit = _mainWindowViewModel.Patient.LastVisit;
            } );
        }

        private void AddPatientVisitCommandHandler( Patient patient )
        {
            _viewProvider.NavigateToPage<AddPatientViewModel>( m =>
            {
                _mainWindowViewModel.Patient = patient ?? SelectedPatient;
                m.Patient = _mainWindowViewModel.Patient;
                m.ActualPatientVisit = m.Patient.AddVisit();
            } );
        }

        private void AddPatientCommandHandler( object _ )
        {
            _viewProvider.NavigateToPage<AddPatientViewModel>( m =>
            {
                m.Patient = Patient.CreateNew( _identityService.CurrentUser.Login );
                m.ActualPatientVisit = m.Patient.AddVisit();
            } );
        }

        #endregion


        #region Initialization

        public PatientsViewModel( IViewProvider viewProvider,
                                  IUnitOfWorkFactory unitOfWorkFactory,
                                  IdentityService identityService,
                                  MainWindowViewModel mainWindowViewModel )
        {
            _viewProvider = viewProvider;
            _unitOfWorkFactory = unitOfWorkFactory;
            _identityService = identityService;
            _mainWindowViewModel = mainWindowViewModel;

            using ( var db = _unitOfWorkFactory.CreateUnitOfWork() )
                Patients = db.PatientsRepository.GetAllPatients().ToList();

            AddPatientCommand = new AsyncDelegateCommand( AddPatientCommandHandler );
            AddPatientVisitCommand = new AsyncDelegateCommand<Patient>( AddPatientVisitCommandHandler );
            EditPatientCommand = new AsyncDelegateCommand<Patient>( EditPatientCommandHandler );
            ShowPatientCommand = new AsyncDelegateCommand<Patient>( ShowPatientCommandHandler );
            ShowPatientStatistics = new AsyncDelegateCommand( ShowPatientStatisticsCommandHandler );
        }

        #endregion


        #region Non-public methods

        private bool PatientsFilter( object o )
        {
            if ( !(o is Patient patient) )
                return false;

            //  Skip if user privileges do not allow viewing patient
            if ( _identityService.CurrentUser.Role != Roles.Admin && patient.RegisteredBy != _identityService.CurrentUser.Login )
                return false;

            //  Filter patient by name
            return patient.Name.IndexOf( PatientFilter, StringComparison.InvariantCultureIgnoreCase ) != -1 ||
                   patient.MiddleName.IndexOf( PatientFilter, StringComparison.InvariantCultureIgnoreCase ) != -1 ||
                   patient.Surname.IndexOf( PatientFilter, StringComparison.InvariantCultureIgnoreCase ) != -1;
        }

        #endregion
    }
}
