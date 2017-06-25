using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using HypertensionControlUI.Models;
using HypertensionControlUI.Utils;
using Newtonsoft.Json;
using SQLite.CodeFirst;

namespace HypertensionControlUI.Services
{
    internal class SqlDbInitializer : SqliteCreateDatabaseIfNotExists<SqlDbContext>
    {
        #region Fields

        private readonly ResourceProvider _resourceProvider;

        #endregion


        #region Initialization

        public SqlDbInitializer( DbModelBuilder modelBuilder, ResourceProvider resourceProvider ) : base( modelBuilder, true )
        {
            _resourceProvider = resourceProvider;
        }

        #endregion


        /// <summary>
        /// Executes the strategy to initialize the database for the given context.
        /// </summary>
        /// <param name="context"> The context. </param>
        public override void InitializeDatabase( SqlDbContext context )
        {
            base.InitializeDatabase( context );
        }


        #region Non-public methods

        protected override void Seed( SqlDbContext context )
        {
            context.Users.Add( new User { Name = "admin", Login = "admin", PasswordHash = HashUtils.GetStringHash( "admin" ), Role = Role.Admin } );

            context.Users.Add( new User
            {
                Name = "�����",
                Surname = "�������",
                MiddleName = "����������",
                PasswordHash = HashUtils.GetStringHash( "password" ),
                Job = new Clinic
                {
                    Name = "��������������� ������-������������ ����� �������������",
                    Address = "���������� ��������, �. �����, ��. �. ����������, 110"
                },
                Position = "���������� ������������ ������������ ����������,\n" +
                           "�������� ����������� ����, ������,\n" +
                           "������ ��������� �� ������������� �����������".Replace( "\n",
                                                                                    Environment.NewLine ),
                Role = Role.Admin,
                Login = "VolhaPaulava"
            } );

            //            var testPatient = new Patient
            //            {
            //                Name = "����",
            //                MiddleName = "������������",
            //                Surname = "����������",
            //                Nationality = "�������",
            //                BirthPlace = "��������, �. �����",
            //                Phone = "+375171234567",
            //                Address = "���������� ��������, �. �����, ��. ������������� 187-35",
            //                BirthDate = new DateTime( 1978, 11, 11 ),
            //                Gender = GenderType.Male,
            //                Genes =
            //                {
            //                    new Gene { Name = "AGT", Value = 2 },
            //                    new Gene { Name = "AGTR2", Value = 3 }
            //                },
            //                PatientVisitHistory = 
            //                {
            //                    new PatientVisitData
            //                        {
            //                            WaistCircumference = 124,
            //                            Height = 182,
            //                            Weight = 120,
            //                            PhysicalActivity = PhysicalActivity.OncePerWeekOrLess,
            //                            SaltSensitivity = new SaltSensitivityTest
            //                            {
            //                                SaltSensitivity = 1,
            //                                TestDate = DateTime.Today
            //                            },
            //                            Alcohol�onsumption = Alcohol�onsumption.Monthly,
            //                            BloodPressure = new BloodPressure
            //                            {
            //                                LeftShoulderSBP = 157,
            //                                LeftShoulderDBP = 113,
            //                                RightShoulderSBP = 168,
            //                                RightShoulderDBP = 111,
            //                                HeartRate = 81
            //                            },
            //                            DepressionPointsCES_D = 37,
            //                            StressPointsPSM_25 = 141,
            //                            Smoking = new Smoking
            //                            {
            //                                Type = SmokingType.Now,
            //                                CigarettesPerDay = 20,
            //                                DurationInYears = 5
            //                            }
            //                        }
            //                },
            //                Clinic = new Clinic
            //                {
            //                    Address = "���������� ��������, �. �����, ��. ����������, �. 3",
            //                    Name = "13 ��������� �����������"
            //                },
            //                Diagnosis = "��������� ��",
            //                TreatmentDuration = 4,
            //                HypertensionDuration = 5,
            //                Medicine = 
            //                {
            //                    new Medicine
            //                    {
            //                        Name = "����������",
            //                        Dose = "25 ��"
            //                    },
            //                    new Medicine
            //                    {
            //                        Name = "����������",
            //                        Dose = "5 ��"
            //                    }
            //                },
            //                HypertensionAncestralAnamnesis = HypertensionAncestralAnamnesis.Father
            //                
            //            };
            //            context.Patients.Add( testPatient );

            var patients = ReadPatients();

            //  Load initial models from the JSON file stored in resources
            List<ClassificationModel> models;
            try
            {
                var modelsJson = _resourceProvider.ReadAllResourceText( "initial_models_data.json" );
                models = JsonConvert.DeserializeObject<List<ClassificationModel>>( modelsJson );
            }
            catch ( Exception )
            {
                models = new List<ClassificationModel>();
            }

            //            var optimalCutOffCalculator = new OptimalCutOffCalculator(new PatientPropertyProvider());
            //
            //            foreach (var model in models)
            //            {
            //                model.OptimalCutOff = optimalCutOffCalculator.CalculateOptimalCutOff(model, patients);
            //            }

            context.ClassificationModels.AddRange( models );
            context.Patients.AddRange( patients );

            context.SaveChanges();
        }

        private List<Patient> ReadPatients()
        {
            //  Try to load the data from the application resources
            try
            {
                var lines = _resourceProvider.ReadAllResourceLines( "initial_patients_data.csv" );

                if ( lines.Length == 0 )
                    return new List<Patient>();

                var patientDictionaries = ReadCsvAsDictionaries( lines );
                return patientDictionaries.Select( PatientParser.ReadPatientFromDictionary ).ToList();
            }
            catch ( Exception ex )
            {
                //  do nothing TODO: Fix
            }

            return new List<Patient>();
        }

        /// <summary>
        ///     Reads CSV file content as a list of dictionaries. Each dictionary represents a single CSV-file line with keys taken
        ///     from the first line of the CSV file (headers).
        /// </summary>
        /// <param name="lines">Collection of CSV file lines as an arary of strings.</param>
        /// <returns>CSV file content as a list of dictionaries.</returns>
        private static IEnumerable<Dictionary<string, string>> ReadCsvAsDictionaries( string[] lines )
        {
            //  Prepare the collection of dictionary keys
            var dictionaryKeys = lines.First().Split( ';' );

            //  Converts a single CSV-file line to a dictionary
            Dictionary<string, string> LineToDictionaryConverter( string line )
            {
                return line.Split( ';' )
                           .Select( ( field, index ) => new { key = dictionaryKeys[index], value = field } )
                           .ToDictionary( pair => pair.key, pair => pair.value );
            }

            //  Process CSV-file lines using the defined converter
            return lines.Skip( 1 ).Select( LineToDictionaryConverter ).ToList();
        }

        #endregion
    }
}
