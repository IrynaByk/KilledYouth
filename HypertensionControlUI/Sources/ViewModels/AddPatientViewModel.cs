using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using HypertensionControl.Domain.Models;
using HypertensionControl.Domain.Models.Values;
using HypertensionControlUI.Interfaces;
using HypertensionControlUI.Utils;

namespace HypertensionControlUI.ViewModels
{
    public class AddPatientViewModel : PageViewModelBase
    {
        #region Fields

        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IViewProvider _viewProvider;
        private Patient _patient;
        private Clinic _selectedClinic;
        private string _selectedClinicAddress;

        #endregion


        #region Auto-properties

        public List<Clinic> Clinics { get; }
        public string SelectedClinicName { get; set; }
        public PatientVisit ActualPatientVisit { get; set; }
        public ICommand PatientsCommand { get; }
        public ICommand AddPatientCommand { get; }
        public ICommand DeleteMedicineCommand { get; }
        public ObservableCollection<Medicine> Medicines { get; set; }
        public bool IsQuestionnaireVisible { get; }
        public ICommand AddMedicineCommand { get; set; }

        #endregion


        #region Properties

        public Patient Patient
        {
            get => _patient;
            set
            {
                _patient = value;
                Medicines = new ObservableCollection<Medicine>( _patient.Medicine );
                SelectedClinic = Clinics.FirstOrDefault( clinic => clinic.Id == Patient.ClinicId );
            }
        }

        public double? TreatmentDuration
        {
            get => Patient.TreatmentDuration;
            set
            {
                if ( Equals( value, Patient.TreatmentDuration ) )
                {
                    return;
                }
                Patient.TreatmentDuration = value;

                OnPropertyChanged( nameof(HasTreatment) );
                OnPropertyChanged();
            }
        }

        public bool HasTreatment
        {
            get => Patient.TreatmentDuration.HasValue;
            set
            {
                if ( HasTreatment == value )
                {
                    return;
                }
                TreatmentDuration = value ? (double?) 0 : null;
            }
        }

        public double Weight
        {
            get => ActualPatientVisit.Weight;
            set
            {
                if ( Equals( value, Weight ) )
                {
                    return;
                }
                ActualPatientVisit.Weight = value;

                OnPropertyChanged( nameof(Bmi) );
                OnPropertyChanged( nameof(ObesityBmi) );
                OnPropertyChanged( nameof(HaveHeightWidthWaist) );
                OnPropertyChanged();
            }
        }

        public double Height
        {
            get => ActualPatientVisit.Height;
            set
            {
                if ( Equals( value, Height ) )
                {
                    return;
                }
                ActualPatientVisit.Height = value;

                OnPropertyChanged( nameof(Bmi) );
                OnPropertyChanged( nameof(ObesityBmi) );
                OnPropertyChanged( nameof(HaveHeightWidthWaist) );
                OnPropertyChanged();
            }
        }

        public double WaistCircumference
        {
            get => ActualPatientVisit.WaistCircumference;
            set
            {
                if ( Equals( value, WaistCircumference ) )
                {
                    return;
                }
                ActualPatientVisit.WaistCircumference = value;

                OnPropertyChanged( nameof(ObesityWaistCircumference) );
                OnPropertyChanged( nameof(HaveHeightWidthWaist) );
                OnPropertyChanged();
            }
        }

        public double? Bmi => ActualPatientVisit.Bmi;

        public SmokingType SmokingType
        {
            get => ActualPatientVisit.Smoking.Type;
            set
            {
                if ( value == SmokingType )
                {
                    return;
                }
                ActualPatientVisit.Smoking.Type = value;

                OnPropertyChanged( nameof(NeverSmoke) );
                OnPropertyChanged( nameof(SmokingNow) );
                OnPropertyChanged( nameof(SmokingBefore) );
                OnPropertyChanged();
            }
        }

        public string ObesityBmi
        {
            get
            {
                if ( ActualPatientVisit.ObesityBmi == null )
                {
                    return "Нет данных";
                }
                return ActualPatientVisit.ObesityBmi == true ? "Есть" : "Нет";
            }
        }

        public string ObesityWaistCircumference
        {
            get
            {
                if ( ActualPatientVisit.ObesityWaistCircumference == null )
                {
                    return "Нет данных";
                }
                return ActualPatientVisit.ObesityWaistCircumference == true ? "Есть" : "Нет";
            }
        }

        public string SelectedClinicAddress
        {
            get => _selectedClinicAddress;
            set => Set( ref _selectedClinicAddress, value );
        }

        public Clinic SelectedClinic
        {
            get => _selectedClinic;
            set
            {
                if ( !Set( ref _selectedClinic, value ) )
                {
                    return;
                }
                if ( _selectedClinic == null )
                {
                    return;
                }

                SelectedClinicAddress = value.Address;
                SelectedClinicName = value.Name;
            }
        }

        public GeneValue Agt
        {
            get => GetGene( GenesNames.Agt );
            set
            {
                if ( Agt == value )
                {
                    return;
                }
                SetGene( GenesNames.Agt, value );
            }
        }

        public GeneValue Agtr2
        {
            get => GetGene( GenesNames.Agtr2 );
            set
            {
                if ( Agtr2 == value )
                {
                    return;
                }
                SetGene( GenesNames.Agtr2, value );
            }
        }

        public bool NeverSmoke
        {
            get => SmokingType == SmokingType.Never;
            set
            {
                if ( !value )
                {
                    return;
                }
                SmokingType = SmokingType.Never;
            }
        }

        public bool SmokingNow
        {
            get => SmokingType == SmokingType.Now;
            set
            {
                if ( !value )
                {
                    return;
                }
                SmokingType = SmokingType.Now;
            }
        }

        public bool SmokingBefore
        {
            get => SmokingType == SmokingType.InPast;
            set
            {
                if ( !value )
                {
                    return;
                }
                SmokingType = SmokingType.InPast;
            }
        }

        public string PatientName
        {
            get => Patient.Name;
            set
            {
                if ( value == Patient.Name )
                {
                    return;
                }
                Patient.Name = value;

                OnPropertyChanged( nameof(HaveNameAndAge) );
                OnPropertyChanged();
            }
        }

        public GenderType PatientGender
        {
            get => Patient.Gender;
            set
            {
                if ( value == Patient.Gender )
                {
                    return;
                }
                Patient.Gender = value;
                OnPropertyChanged();
            }
        }

        public string PatientMiddleName
        {
            get => Patient.MiddleName;
            set
            {
                if ( value == Patient.MiddleName )
                {
                    return;
                }
                Patient.MiddleName = value;
                OnPropertyChanged();
                OnPropertyChanged( nameof(HaveNameAndAge) );
            }
        }

        public string PatientSurname
        {
            get => Patient.Surname;
            set
            {
                if ( value == Patient.Surname )
                {
                    return;
                }
                Patient.Surname = value;
                OnPropertyChanged();
                OnPropertyChanged( nameof(HaveNameAndAge) );
            }
        }

        public DateTime PatientBirthDate
        {
            get => Patient.BirthDate;
            set
            {
                if ( value.Equals( Patient.BirthDate ) )
                {
                    return;
                }
                Patient.BirthDate = value;
                OnPropertyChanged();
                OnPropertyChanged( nameof(HaveNameAndAge) );
            }
        }

        public bool HaveNameAndAge => !string.IsNullOrEmpty( Patient.Name ) &&
                                      !string.IsNullOrEmpty( Patient.Surname ) &&
                                      Patient.Age > 0 && Patient.Age < 120;

        public bool HaveHeightWidthWaist => ActualPatientVisit.Weight > 0 &&
                                            ActualPatientVisit.Height > 0 &&
                                            ActualPatientVisit.WaistCircumference > 0;

        #endregion


        #region Commands

        private void DeleteMedicineCommandHandler( object obj )
        {
            var medicine = obj as Medicine;
            if ( medicine != null )
            {
                var result = Medicines.Remove( medicine );
            }
        }

        private void AddMedicineCommandHandler( object obj )
        {
            Medicines.Add( new Medicine() );
        }

        private void AddPatientCommandHandler( object o )
        {
            using ( var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork() )
            {
                if ( SelectedClinic != null )
                {
                    Patient.ClinicId = SelectedClinic.Id;
                }
                else
                {
                    var clinic = Clinic.CreateNew( SelectedClinicAddress, SelectedClinicName );
                    unitOfWork.ClinicsRepository.SaveClinic( clinic );
                    Patient.ClinicId = clinic.Id;
                }

                Patient.Medicine = Medicines;

                unitOfWork.PatientsRepository.SavePatient( Patient );

                unitOfWork.SaveChanges();
            }
            _mainWindowViewModel.Patient = Patient;
            _viewProvider.NavigateToPage<IndividualPatientCardViewModel>( model =>
            {
                model.Patient = Patient;
                model.PatientVisit = ActualPatientVisit;
            } );
        }

        #endregion


        #region Initialization

        public AddPatientViewModel( MainWindowViewModel mainWindowViewModel, IViewProvider viewProvider, IUnitOfWorkFactory unitOfWorkFactory )
        {
            IsQuestionnaireVisible = false;
            _mainWindowViewModel = mainWindowViewModel;
            _viewProvider = viewProvider;
            _unitOfWorkFactory = unitOfWorkFactory;

            using ( var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork() )
            {
                Clinics = unitOfWork.ClinicsRepository.GetAllClinics().OrderBy( clinic => clinic.Name ).ToList();
            }

            AddPatientCommand = new AsyncDelegateCommand( AddPatientCommandHandler );
            DeleteMedicineCommand = new AsyncDelegateCommand( DeleteMedicineCommandHandler );
            AddMedicineCommand = new AsyncDelegateCommand( AddMedicineCommandHandler );
            PatientsCommand = new AsyncDelegateCommand( o => _viewProvider.NavigateToPage<PatientsViewModel>( m => _mainWindowViewModel.Patient = null ) );
        }

        #endregion


        #region Non-public methods

        private GeneValue GetGene( string geneName )
        {
            return Patient.GetGeneValue( geneName );
        }

        private void SetGene( string geneName, GeneValue value )
        {
            Patient.SetGeneValue( geneName, value );
        }

        #endregion
    }
}
