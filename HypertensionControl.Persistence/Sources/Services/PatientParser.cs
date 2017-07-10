using System;
using System.Collections.Generic;
using System.Globalization;
using HypertensionControl.Domain.Models;
using HypertensionControl.Domain.Models.Values;
using HypertensionControl.Persistence.Entities;
using Newtonsoft.Json;

namespace HypertensionControl.Persistence.Services
{
    internal static class PatientParser
    {
        #region Public methods

        internal static PatientEntity ReadPatientFromDictionary( IDictionary<string, string> patientProperties )
        {
            var ruCulture = new CultureInfo( "RU-ru" );

            //  Create patient instance

            var patient = new PatientEntity
            {
                Id = Guid.NewGuid(),
                VisitHistory = new List<PatientVisitEntity>()
            };

            patient.TreatmentDuration = double.TryParse( patientProperties["HDurability"], out var duration ) ? duration : 0;

            patient.HypertensionAncestralAnamnesis = int.TryParse( patientProperties["Hheredity"], out var anamnesis )
                ? (HypertensionAncestralAnamnesis) anamnesis
                : HypertensionAncestralAnamnesis.None;

            var nameParts = patientProperties["name"].Split( ' ' );
            patient.Name = nameParts[1];
            patient.Surname = nameParts[0];
            patient.MiddleName = nameParts[2];

            patient.BirthDateTicks = (DateTime.Today - TimeSpan.FromDays( Convert.ToInt32( patientProperties["age"] ) * 365 )).Ticks;

            patient.Gender = patientProperties["gender"].IndexOfAny( new[] { 'ж', 'Ж' } ) != -1 ? GenderType.Female : GenderType.Male;

            patient.MedicineSerialized = "[]";

            if ( !string.IsNullOrEmpty( patientProperties["MaleHeredity"] ) )
                patient.MaleHeredity = Convert.ToBoolean( Convert.ToInt32( patientProperties["MaleHeredity"] ) );

            if ( !string.IsNullOrEmpty( patientProperties["FemaleHeredity"] ) )
                patient.FemaleHeredity = Convert.ToBoolean( Convert.ToInt32( patientProperties["FemaleHeredity"] ) );

            var genes = new Dictionary<string, Gene>();

            if ( !string.IsNullOrEmpty( patientProperties[GenesNames.Agt] ) )
                genes[GenesNames.Agt] = new Gene(GenesNames.Agt, Convert.ToInt32( patientProperties[GenesNames.Agt] ) );
            if ( !string.IsNullOrEmpty( patientProperties[GenesNames.Agtr2] ) )
                genes[GenesNames.Agtr2] = new Gene(GenesNames.Agtr2, Convert.ToInt32( patientProperties[GenesNames.Agtr2] ) );

            patient.GenesSerialized = JsonConvert.SerializeObject( genes );

            //  Create patient visit instance

            var patientVisit = new PatientVisitEntity{VisitDateTicks = new DateTime(2015, 1, 1).Ticks };

            Smoking smoking;
            if ( patientProperties["smoke"].Contains( "1" ) )
                smoking = new Smoking{Type = SmokingType.Never };
            else if ( patientProperties["smoke"].Contains( "2" ) )
                smoking = new Smoking { Type = SmokingType.InPast};
            else
                smoking = new Smoking { Type = SmokingType.Now};

            patientVisit.SmokingSeralized = JsonConvert.SerializeObject( smoking );

            patientVisit.BloodPressureSerialized = "";
            patientVisit.DietaryHabitsSerialized = "";
            patientVisit.SaltSensitivitySerialized = "";

            if ( patientProperties["HStage"].Contains( "1" ) )
                patientVisit.HypertensionStage = HypertensionStage.Stage1;
            if ( patientProperties["HStage"].Contains( "2" ) )
                patientVisit.HypertensionStage = HypertensionStage.Stage2;
            if ( patientProperties["HStage"].Contains( "3" ) )
                patientVisit.HypertensionStage = HypertensionStage.Stage3;
            if ( patientProperties["HStage"].Contains( "зд" ) )
                patientVisit.HypertensionStage = HypertensionStage.Healthy;

            if ( !string.IsNullOrEmpty( patientProperties["WaistCircumference"] ) )
                patientVisit.WaistCircumference = Convert.ToDouble( patientProperties["WaistCircumference"], ruCulture );

            if (!string.IsNullOrEmpty(patientProperties["Height"]))
                patientVisit.Height = Convert.ToDouble(patientProperties["Height"]);
            if (!string.IsNullOrEmpty(patientProperties["Weight"]))
                patientVisit.Weight = Convert.ToDouble(patientProperties["Weight"]);

            if ( patientProperties["HStage"].Contains( "1" ) )
                patientVisit.HypertensionStage = HypertensionStage.Stage1;
            else if ( patientProperties["HStage"].Contains( "2" ) )
                patientVisit.HypertensionStage = HypertensionStage.Stage2;
            else if ( patientProperties["HStage"].Contains( "3" ) )
                patientVisit.HypertensionStage = HypertensionStage.Stage3;
            else
                patientVisit.HypertensionStage = HypertensionStage.Healthy;

            if ( !string.IsNullOrEmpty( patientProperties["phiz"] ) )
            {
                var phiz = Convert.ToInt32( patientProperties["phiz"] );
                if ( phiz == 4 )
                    patientVisit.PhysicalActivity = PhysicalActivity.MoreThenThreeTimesPerWeek;
                else if ( phiz == 3 )
                    patientVisit.PhysicalActivity = PhysicalActivity.FromOneToThreeTimesPerWeek;
                else if ( phiz == 2 )
                    patientVisit.PhysicalActivity = PhysicalActivity.OncePerWeekOrLess;
                else
                    patientVisit.PhysicalActivity = PhysicalActivity.Never;
            }

            //  Compose patient and return
            patientVisit.PatientId = patient.Id;
            patient.VisitHistory.Add( patientVisit );
            return patient;
        }

        #endregion
    }
}
