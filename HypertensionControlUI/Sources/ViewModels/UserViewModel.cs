using System.Collections.Generic;
using System.Linq;
using HypertensionControlUI.CompositionRoot;
using HypertensionControlUI.Models;

namespace HypertensionControlUI.ViewModels
{
    public class UserViewModel : PageViewModelBase
    {
        #region Fields

        private readonly DbContextFactory _dbContextFactory;
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly IViewProvider _viewProvider;
        private Clinic _selectedClinic;
        private string _selectedClinicAddress;
        private User _user;

        #endregion


        #region Auto-properties

        public List<Clinic> Clinics { get; set; }
        public string SelectedClinicName { get; set; }

        #endregion


        #region Properties

        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                SelectedClinic = User.Job;
            }
        }

        public string SelectedClinicAddress
        {
            get { return _selectedClinicAddress; }
            set
            {
                if ( value == _selectedClinicAddress )
                    return;
                _selectedClinicAddress = value;
                OnPropertyChanged();
            }
        }

        public Clinic SelectedClinic
        {
            get { return _selectedClinic; }
            set
            {
                if ( _selectedClinic == value )
                    return;
                _selectedClinic = value;

                if ( value != null )
                {
                    SelectedClinicAddress = value.Address;
                    SelectedClinicName = value.Name;
                }

                OnPropertyChanged();
            }
        }

        public bool HaveNameAndAge
        {
            get
            {
                return !string.IsNullOrEmpty( User.Name ) &&
                       !string.IsNullOrEmpty( User.Surname ) &&
                       !string.IsNullOrEmpty( User.Position ) &&
                       User.Job != null &&
                       !string.IsNullOrEmpty( User.Job.Name ) &&
                       !string.IsNullOrEmpty( User.Job.Address ) &&
                       !string.IsNullOrEmpty( User.PasswordHash );
            }
        }

        #endregion


        #region Initialization

        public UserViewModel( DbContextFactory dbContextFactory, MainWindowViewModel mainWindowViewModel, IViewProvider viewProvider )
        {
            _dbContextFactory = dbContextFactory;
            _mainWindowViewModel = mainWindowViewModel;
            _viewProvider = viewProvider;
            using ( var db = _dbContextFactory.GetDbContext() )
                Clinics = db.Clinics.ToList();
        }

        #endregion
    }
}
