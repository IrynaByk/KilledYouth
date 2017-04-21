using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using HypertensionControlUI.Models;
using HypertensionControlUI.Utils;
using SQLite.CodeFirst;

namespace HypertensionControlUI.Services
{
    internal class SqlDbInitializer : SqliteCreateDatabaseIfNotExists<SqlDbContext>
    {
        public SqlDbInitializer(DbModelBuilder modelBuilder) : base(modelBuilder)
        {
        }

        public SqlDbInitializer(DbModelBuilder modelBuilder, bool nullByteFileMeansNotExisting) : base(modelBuilder, nullByteFileMeansNotExisting)
        {
        }

        #region Public methods

        public override void InitializeDatabase(SqlDbContext context)
        {
            base.InitializeDatabase(context);
//            Seed(context);
        }

        protected override void Seed(SqlDbContext context)
        {
            context.Users.Add(new User {Name = "admin", Login = "admin", PasswordHash = HashUtils.GetStringHash("admin"), Role = Role.Admin});
            context.Users.Add(new User
                              {
                                  Name = "Ольга",
                                  Surname = "Павлова",
                                  MiddleName = "Степановна",
                                  PasswordHash = HashUtils.GetStringHash("password"),
                                  Job = new Clinic
                                        {
                                            Name = "Республиканский научно-практический центр «Кардиология»",
                                            Address = "Республика Беларусь, г. Минск, ул. Р. Люксембург, 110"
                                        },
                                  Position = "Заведующая лабораторией артериальной гипертонии, " +
                                             "кандидат медицинских наук, доцент, " +
                                             "высшая категория по специальности кардиология",
                                  Role = Role.User,
                                  Login = "VolhaPaulava"
                              });

            //            var testPatient = new Patient
            //            {
            //                Name = "Юрий",
            //                MiddleName = "Валентинович",
            //                Surname = "Петрашевич",
            //                Nationality = "белорус",
            //                BirthPlace = "Беларусь, г. Минск",
            //                Phone = "+375171234567",
            //                Address = "Республика Беларусь, г. Минск, пр. Независимости 187-35",
            //                BirthDate = new DateTime( 1978, 11, 11 ),
            //                Gender = GenderType.Male,
            //                Genes =
            //                {
            //                    new Gene { Name = "AGT", Value = 2 },
            //                    new Gene { Name = "AGTR2", Value = 3 }
            //                },
            //                PatientVisitDataHistory = 
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
            //                            AlcoholСonsumption = AlcoholСonsumption.Monthly,
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
            //                    Address = "Республика Беларусь, г. Минск, ул. Ломоносова, д. 3",
            //                    Name = "13 городская поликлиника"
            //                },
            //                Diagnosis = "Первичная АГ",
            //                TreatmentDuration = 4,
            //                HypertensionDuration = 5,
            //                Medicine = 
            //                {
            //                    new Medicine
            //                    {
            //                        Name = "Карведилол",
            //                        Dose = "25 мг"
            //                    },
            //                    new Medicine
            //                    {
            //                        Name = "Бисопролол",
            //                        Dose = "5 мг"
            //                    }
            //                },
            //                HypertensionAncestralAnamnesis = HypertensionAncestralAnamnesis.Father
            //                
            //            };
            //            context.Patients.Add( testPatient );

            var patients = ReadPatients();

            var models = new List<ClassificationModel>
                         {
                             new ClassificationModel
                             {
                                 LimitPoints = new List<LimitPoint> {new LimitPoint {Point = 0.5}},
                                 Name = "TestModel",
                                 FreeCoefficient = -1.193050,
                                 Properties = new List<ModelProperty>
                                              {
                                                  new ModelProperty
                                                  {
                                                      Name = "Age",
                                                      Entries = new List<ModelScaleEntry>
                                                                {
                                                                    new ModelScaleEntry {LowerBound = 45, Value = 1}
                                                                },
                                                      ModelCoefficient = 1.095915
                                                  },
                                                  new ModelProperty
                                                  {
                                                      Name = "{PatientVisitData}.ObesityWaistCircumference",
                                                      Entries = new List<ModelScaleEntry>(),
                                                      ModelCoefficient = 0.637895
                                                  },
                                                  new ModelProperty
                                                  {
                                                      Name = "{PatientVisitData}.ObesityBMI",
                                                      Entries = new List<ModelScaleEntry>(),
                                                      ModelCoefficient = 1.380139
                                                  },
                                                  new ModelProperty
                                                  {
                                                      Name = "Gender",
                                                      Entries = new List<ModelScaleEntry>(),
                                                      ModelCoefficient = 0.668418
                                                  },
                                                  new ModelProperty
                                                  {
                                                      Name = "{PatientVisitData}.PhysicalActivity",
                                                      Entries = new List<ModelScaleEntry>(),
                                                      ModelCoefficient = 0.668418
                                                  },
                                                  new ModelProperty
                                                  {
                                                      Name = "AGT_AGTR2",
                                                      Entries = new List<ModelScaleEntry>(),
                                                      ModelCoefficient = 0.423879
                                                  }
                                              }
                             },
                             new ClassificationModel
                             {
                                 LimitPoints = new List<LimitPoint> {new LimitPoint {Point = 0.5}},
                                 Name = "TestModelNoGene",
                                 FreeCoefficient = -0.819416,
                                 Properties = new List<ModelProperty>
                                              {
                                                  new ModelProperty
                                                  {
                                                      Name = "Age",
                                                      Entries = new List<ModelScaleEntry>
                                                                {
                                                                    new ModelScaleEntry {LowerBound = 45, Value = 1}
                                                                },
                                                      ModelCoefficient = 0.973776
                                                  },
                                                  new ModelProperty
                                                  {
                                                      Name = "{PatientVisitData}.ObesityWaistCircumference",
                                                      Entries = new List<ModelScaleEntry>(),
                                                      ModelCoefficient = 0.582647
                                                  },
                                                  new ModelProperty
                                                  {
                                                      Name = "{PatientVisitData}.ObesityBMI",
                                                      Entries = new List<ModelScaleEntry>(),
                                                      ModelCoefficient = 1.541386
                                                  },
                                                  new ModelProperty
                                                  {
                                                      Name = "{PatientVisitData}.PhysicalActivity",
                                                      Entries = new List<ModelScaleEntry>(),
                                                      ModelCoefficient = 0.708104
                                                  },
                                                  new ModelProperty
                                                  {
                                                      Name = "{PatientVisitData}.Smoking.Type",
                                                      Entries = new List<ModelScaleEntry>(),
                                                      ModelCoefficient = 0.423879
                                                  }
                                              }
                             }
                         };

            var optimalCutOffCalculator = new OptimalCutOffCalculator(new PatientPropertyProvider());

            foreach (var model in models)
            {
                model.OptimalCutOff = optimalCutOffCalculator.CalculateOptimalCutOff(model, patients);
            }

            context.ClassificationModels.AddRange(models);
            context.Patients.AddRange(patients);


            context.SaveChanges();
        }

        private static List<Patient> ReadPatients()
        {
            //  Call once!
            const string filePath = @"c:\Users\Asaniel\Documents\last_base_fixed.csv";

            try
            {
                var lines = File.ReadAllLines(filePath);
                if (lines.Length == 0)
                    return new List<Patient>();

                var dictionaryKeys = lines.First().Split(';');

                var patientDictionaries =
                    lines.Skip(1)
                         .Select(line => line.Split(';')
                                             .Select((field, index) => new {key = dictionaryKeys[index], value = field})
                                             .ToDictionary(pair => pair.key, pair => pair.value));

                return patientDictionaries.Select(dict => PatientParser.ReadPatientFromDictionary(dict)).ToList();
            }
            catch (Exception ex)
            {
                //  do nothing TODO: Fix
            }

            return new List<Patient>();
        }

        #endregion
    }
}