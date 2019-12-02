using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using HypertensionControl.Domain.Interfaces;
using HypertensionControl.Domain.Models;
using HypertensionControl.Persistence.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite.CodeFirst;

namespace HypertensionControl.Persistence.Services
{
    internal class SqlDbInitializer : SqliteCreateDatabaseIfNotExists<SqliteDbContext>
    {
        #region Fields

        private readonly IResourceProvider _resourceProvider;

        #endregion


        #region Initialization

        public SqlDbInitializer( DbModelBuilder modelBuilder, IResourceProvider resourceProvider ) : base( modelBuilder, true )
        {
            _resourceProvider = resourceProvider;
        }

        #endregion


        #region Non-public methods

        protected override void Seed( SqliteDbContext context )
        {
            //  Users

            context.Users.Add( new UserEntity { Id = Guid.NewGuid().ToString(), Name = "admin", Login = "admin", PasswordHash = HashUtils.GetStringHash( "admin" ), Role = Roles.Admin } );

            context.Users.Add( new UserEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Ольга",
                Surname = "Павлова",
                MiddleName = "Степановна",
                PasswordHash = HashUtils.GetStringHash( "password" ),
                ClinicName = "Республиканский научно-практический центр «Кардиология»",
                ClinicAddress = "г. Минск",
                Position = "Заведующая лабораторией артериальной гипертонии,\n" +
                           "кандидат медицинских наук, доцент,\n" +
                           "высшая категория по специальности кардиология".Replace( "\n",
                                                                                    Environment.NewLine ),
                Role = Roles.Admin,
                Login = "VolhaPaulava"
            } );
            context.Users.Add(new UserEntity
                              {
                                  Id = Guid.NewGuid().ToString(),
                                  Name = "Инна",
                                  Surname = "",
                                  MiddleName = "Викторовна",
                                  PasswordHash = HashUtils.GetStringHash("password"),
                                  ClinicName = "«30-я городская клиническая поликлиника»",
                                  ClinicAddress = "г. Минск",
                                  Position = "Главврач",
                                  Role = Roles.User,
                                  Login = "InnaViktorovna"
                              });
            //  Patients

            var patients = ReadPatients();
            context.Patients.AddRange( patients );

            //  Load initial models from the JSON file stored in resources
            List<ClassificationModelEntity> models;
            try
            {
                var modelsJson = _resourceProvider.ReadAllResourceText( "initial_models_data.json" );
                models = JsonConvert.DeserializeObject<List<ClassificationModelEntity>>( modelsJson, new ClassificationModelEntityConverter() );
            }
            catch ( Exception )
            {
                models = new List<ClassificationModelEntity>();
            }

            //            var optimalCutOffCalculator = new OptimalCutOffCalculator(new PatientPropertyProvider());
            //
            //            foreach (var model in models)
            //            {
            //                model.OptimalCutOff = optimalCutOffCalculator.CalculateOptimalCutOff(model, patients);
            //            }

            context.ClassificationModels.AddRange( models );

            context.SaveChanges();
        }

        private List<PatientEntity> ReadPatients()
        {
            //  Try to load the data from the application resources
            try
            {
                var lines = _resourceProvider.ReadAllResourceLines( "initial_patients_data.csv" );

                if ( lines.Length == 0 )
                    return new List<PatientEntity>();

                var patientDictionaries = ReadCsvAsDictionaries( lines );
                return patientDictionaries.Select( PatientParser.ReadPatientFromDictionary ).ToList();
            }
            catch ( Exception ex )
            {
                //  do nothing TODO: Fix
            }

            return new List<PatientEntity>();
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


        #region Nested type: ClassificationModelEntityConverter

        private class ClassificationModelEntityConverter : JsonConverter
        {
            #region Properties

            public override bool CanWrite => false;

            #endregion


            #region Public methods

            public override bool CanConvert( Type objectType )
            {
                return objectType == typeof(ClassificationModelEntity);
            }

            public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer )
            {
                // Load the JSON for the ClassificationModelEntity into a JObject
                var jObject = JObject.Load( reader );

                // Read the properties which will be used as constructor parameters
                var name = (string) jObject[nameof(ClassificationModel.Name)];
                var description = (string) jObject[nameof(ClassificationModel.Description)];
                var freeCoefficient = (double) jObject[nameof(ClassificationModel.FreeCoefficient)];
                var optimalCutOff = (double) jObject[nameof(ClassificationModel.OptimalCutOff)];
                var limitPoints = jObject["LimitPoints"].ToString(Formatting.None);
                var properties = jObject[nameof(ClassificationModel.Properties)].ToString(Formatting.Indented);

                // Construct the ClassificationModelEntity object using the non-default constructor
                var classificationModelEntity = new ClassificationModelEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = name,
                    Description = description,
                    FreeCoefficient = freeCoefficient,
                    OptimalCutOff = optimalCutOff,
                    LimitPointsSerialized = limitPoints,
                    PropertiesSerialized = properties
                };

                // Return the result
                return classificationModelEntity;
            }

            public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer )
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion
    }
}
