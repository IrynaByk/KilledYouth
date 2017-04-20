using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows.Input;
using HypertensionControlUI.CompositionRoot;
using HypertensionControlUI.Models;
using HypertensionControlUI.Utils;

namespace HypertensionControlUI.ViewModels
{
    public class PatientsViewModel : PageViewModelBase
    {
        #region Fields

        private readonly DbContextFactory _dbContextFactory;
        private readonly IViewProvider _viewProvider;
        private Patient _selectedPatient;

        #endregion


        #region Auto-properties

        public ICommand AddPatientCommand { get; private set; }
        public List<Patient> Patients { get; set; }
        public AsyncDelegateCommand EditPatientCommand { get; set; }
        public AsyncDelegateCommand ShowPatientCommand { get; set; }

        #endregion


        #region Properties

        public Patient SelectedPatient
        {
            get { return _selectedPatient; }
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

        public PatientsViewModel( IViewProvider viewProvider, DbContextFactory dbContextFactory )
        {
            _viewProvider = viewProvider;
            _dbContextFactory = dbContextFactory;
            using ( var db = _dbContextFactory.GetDbContext() )
                Patients = db.Patients
                    .Include(p => p.PatientVisitDataHistory)
                    .Include( p => p.Clinic )
                    .Include( p => p.Medicine  )
                    .Include(p => p.Genes).ToList();
            AddPatientCommand = new AsyncDelegateCommand( o => _viewProvider.NavigateToPage<AddPatientViewModel>( m =>
            {
                m.Patient = new Patient();
                m.ActualPatientVisitData = new PatientVisitData { Patient = m.Patient };

            } ) );
            EditPatientCommand = new AsyncDelegateCommand( o => _viewProvider.NavigateToPage<AddPatientViewModel>( m =>
            {
                m.Patient = SelectedPatient;
                m.ActualPatientVisitData =
                    SelectedPatient
                     .PatientVisitDataHistory
                     .OrderByDescending( pvd => pvd.VisitData )
                     .First();
            } ) );
            ShowPatientCommand = new AsyncDelegateCommand(o => _viewProvider.NavigateToPage<IndividualPatientCardViewModel>(m =>
            {
                m.Patient = SelectedPatient;
                m.PatientVisitData =
                    SelectedPatient
                     .PatientVisitDataHistory
                     .OrderByDescending( pvd => pvd.VisitData )
                     .First();
            } ) );
            
        }

        #endregion


 
    }

}


