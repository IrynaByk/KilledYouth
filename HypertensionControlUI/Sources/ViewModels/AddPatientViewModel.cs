using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Windows.Input;
using HypertensionControlUI.CompositionRoot;
using HypertensionControlUI.Models;
using HypertensionControlUI.Utils;
using HypertensionControlUI.Views.Converters;

namespace HypertensionControlUI.ViewModels
{
    public class AddPatientViewModel : PageViewModelBase
    {
        #region Fields

        private readonly DbContextFactory _dbContextFactory;
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly IViewProvider _viewProvider;
        private Patient _patient;
        private Clinic _selectedClinic;
        private string _selectedClinicAddress;

        #endregion


        #region Auto-properties

        public List<Clinic> Clinics { get; set; }
        public PatientVisitData ActualPatientVisitData { get; set; }
        public string SelectedClinicName { get; set; }
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
                SelectedClinic = Patient.Clinic;
            }
        }

        public double? TreatmentDuration
        {
            get => Patient.TreatmentDuration;
            set
            {
                if ( value == TreatmentDuration )
                    return;
                Patient.TreatmentDuration = value;
                OnPropertyChanged(nameof(HasTreatment));
                OnPropertyChanged();
            }
        }

        public double Weight
        {
            get => ActualPatientVisitData.Weight;
            set
            {
                if (value == Weight)
                    return;
                ActualPatientVisitData.Weight = value;
                OnPropertyChanged(nameof(BMI));
                OnPropertyChanged(nameof(ObesityBMI));
                OnPropertyChanged(nameof(HaveHeightWidthWaist));
                OnPropertyChanged();
            }
        }
        public double Height
        {
            get => ActualPatientVisitData.Height;
            set
            {
                if (value == Height)
                    return;
                ActualPatientVisitData.Height = value;
                OnPropertyChanged(nameof(BMI));
                OnPropertyChanged(nameof(ObesityBMI));
                OnPropertyChanged(nameof(HaveHeightWidthWaist));
                OnPropertyChanged();
            }
        }
        public double WaistCircumference
        {
            get => ActualPatientVisitData.WaistCircumference;
            set
            {
                if (value == WaistCircumference)
                    return;
                ActualPatientVisitData.WaistCircumference = value;
                OnPropertyChanged(nameof(ObesityWaistCircumference));
                OnPropertyChanged(nameof(HaveHeightWidthWaist));
                OnPropertyChanged();
            }
        }
        public double? BMI => ActualPatientVisitData.BMI;

        public string ObesityBMI
        {
            get
            {
                if (ActualPatientVisitData.ObesityBMI != null)
                {
                    return (ActualPatientVisitData.ObesityBMI == true ) ? "Есть" : "Нет";
                }
                return "Нет данных";
            }
        
        } 
        public string ObesityWaistCircumference
        {
            get
            {
                if (ActualPatientVisitData.ObesityWaistCircumference != null)
                {
                    return (ActualPatientVisitData.ObesityWaistCircumference == true) ? "Есть" : "Нет";
                }
                return "Нет данных";
            }

        }
        

        public SmokingType SmokingType
        {
            get => ActualPatientVisitData.Smoking.Type;
            set
            {
                if (value == SmokingType)
                    return;
                ActualPatientVisitData.Smoking.Type = value;
                OnPropertyChanged(nameof(NeverSmoke));
                OnPropertyChanged(nameof(SmokingNow));
                OnPropertyChanged(nameof(SmokingBefore));
                OnPropertyChanged();
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

        public bool HasTreatment
        {
            get => Patient.TreatmentDuration.HasValue;
            set
            {
                if ( HasTreatment == value )
                    return;
                TreatmentDuration = value ? (double?) 0 : null;
                
            }
        }

        public GeneValue AGT
        {
            get { return GetGene("AGT"); }
            set
            {
                if (AGT == value)
                    return;
                SetGene( "AGT", value );
            }
        }
        public GeneValue AGTR2
        {
            get { return GetGene("AGTR2"); }
            set
            {
                if (AGTR2 == value)
                    return;
                SetGene("AGTR2", value);
            }
        }

        private GeneValue GetGene(string geneName)
        {
            var value = Patient.Genes.FirstOrDefault( g => g.Name == geneName )?.Value;
            switch (value)
            {
                case null:
                    return GeneValue.None;
               default:
                    return (GeneValue)value;
            }
        }

        private void SetGene( string geneName, GeneValue value )
        {
            if ( value == GeneValue.None )
            {
                Patient.Genes.Remove( Patient.Genes.FirstOrDefault( g => g.Name == geneName ) );
                return;
            }
            var gene = Patient.Genes.FirstOrDefault( g => g.Name == geneName );
            if ( gene == null )
            {
                gene = new Gene { Name = geneName, Value = Convert.ToInt32( value ) };
                Patient.Genes.Add( gene );
            }
            else
                gene.Value = Convert.ToInt32(value);

        }

        
        public bool NeverSmoke
        {
            get => SmokingType == SmokingType.Never;
            set
            {
                if ( !value )
                    return;
                SmokingType = SmokingType.Never;
            }
        }

        public bool SmokingNow
        {
            get => SmokingType == SmokingType.Now;
            set
            {
                if ( !value )
                    return;
                SmokingType = SmokingType.Now;
            }
        }

        public bool SmokingBefore
        {
            get => SmokingType == SmokingType.InPast;
            set
            {
                if ( !value )
                    return;
                SmokingType = SmokingType.InPast;
            }
        }

        public string PatientName
        {
            get => Patient.Name;
            set
            {
                if ( value == Patient.Name )
                    return;
                Patient.Name = value;
                OnPropertyChanged();
                OnPropertyChanged( nameof(HaveNameAndAge) );
            }
        }
        public GenderType PatientGender
        {
            get => Patient.Gender;
            set
            {
                if (value == Patient.Gender)
                    return;
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
                    return;
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
                    return;
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
                    return;
                Patient.BirthDate = value;
                OnPropertyChanged();
                OnPropertyChanged( nameof(HaveNameAndAge) );
            }
        }

        public bool HaveNameAndAge => !string.IsNullOrEmpty( Patient.Name ) &&
                                      !string.IsNullOrEmpty( Patient.Surname ) &&
                                      Patient.Age > 0 && Patient.Age < 120;

        public bool HaveHeightWidthWaist => (ActualPatientVisitData.Weight > 0) &&
                                            (ActualPatientVisitData.Height > 0) &&
                                            (ActualPatientVisitData.WaistCircumference > 0);

        #endregion


        #region Initialization

        public AddPatientViewModel( MainWindowViewModel mainWindowViewModel, IViewProvider viewProvider, DbContextFactory dbContextFactory )
        {
            IsQuestionnaireVisible = false;
            _mainWindowViewModel = mainWindowViewModel;
            _viewProvider = viewProvider;
            _dbContextFactory = dbContextFactory;

            using ( var db = _dbContextFactory.GetDbContext() )
                Clinics = db.Clinics.ToList();
            AddPatientCommand = new AsyncDelegateCommand( AddPatientCommandHandler );
            DeleteMedicineCommand = new AsyncDelegateCommand( DeleteMedicineCommandHandler );
            AddMedicineCommand = new AsyncDelegateCommand( AddMedicineCommandHandler );
            PatientsCommand = new AsyncDelegateCommand( o => _viewProvider.NavigateToPage<PatientsViewModel>( m =>
                                                            {
                                                                _mainWindowViewModel.Patient = null;
                                                            }
                                                        ) );
        }

        #endregion


        #region Non-public methods

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
            using ( var db = _dbContextFactory.GetDbContext() )
            {
                Patient.Medicine.Clear();
                Patient.Medicine = Medicines;
                if ( SelectedClinic != null )
                {
                    Patient.Clinic = SelectedClinic;
                    db.Attach( Patient.Clinic );
                }
                else
                    Patient.Clinic = new Clinic { Address = SelectedClinicAddress, Name = SelectedClinicName };

                if ( Patient.Id != 0 )

                    //                    db.Attach( Patient );
                    ;else
                    db.Patients.Add( Patient );

                if ( ActualPatientVisitData.Id != 0 )
                    db.Attach( ActualPatientVisitData );
                else
                    Patient.PatientVisitDataHistory.Add( ActualPatientVisitData );

                db.SaveChanges();
            }
            _mainWindowViewModel.Patient = Patient;
            _viewProvider.NavigateToPage<IndividualPatientCardViewModel>( model =>
            {
                model.Patient = Patient;
                model.PatientVisitData = ActualPatientVisitData;
            } );
        }

        #endregion
    }

    public enum GeneValue
    {
        None,
        One,
        Two,
        Three

    }
}
