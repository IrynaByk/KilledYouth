using System.Collections.Generic;
using System.Linq;
using HypertensionControl.Domain.Models;
using HypertensionControlUI.Interfaces;

namespace HypertensionControlUI.ViewModels
{
    public class UserViewModel : PageViewModelBase
    {
        #region Fields

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
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
            get => _user;
            set
            {
                _user = value;
                SelectedClinic = Clinics.FirstOrDefault( clinic => clinic.Id == _user.ClinicId );
            }
        }

        public string SelectedClinicAddress
        {
            get => _selectedClinicAddress;
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
            get => _selectedClinic;
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

        #endregion


        #region Initialization

        public UserViewModel( IUnitOfWorkFactory unitOfWorkFactory )
        {
            _unitOfWorkFactory = unitOfWorkFactory;

            using ( var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork() )
                Clinics = unitOfWork.ClinicsRepository.GetAllClinics().OrderBy( c => c.Name ).ToList();
        }

        #endregion
    }
}
