using System;
using System.Collections.Generic;
using System.Globalization;
using HypertensionControlUI.Models;

namespace HypertensionControlUI.Utils
{
    public static class PatientParser
    {
        #region Public methods

        public static Patient ReadPatientFromDictionary( IDictionary<string, string> patientProperties )
        {
            var patient = new Patient();
            var patientVisitData = new PatientVisitData { Patient = patient };

            var ruCulture = new CultureInfo( "RU-ru" );

            patient.PatientVisitHistory.Add( patientVisitData );

            if ( !string.IsNullOrEmpty( patientProperties["HDurability"] ) )
            {
                double duration = 0;
                try
                {
                    duration = Convert.ToDouble( patientProperties["HDurability"], ruCulture );
                }
                catch ( Exception e )
                {
                }
                patient.TreatmentDuration = duration;
            }
            else
            {
                patient.TreatmentDuration = 0;
            }

            if ( !string.IsNullOrEmpty( patientProperties["Hheredity"] ) )
            {
                var hHeredity = Convert.ToInt32( patientProperties["Hheredity"] );
                if ( hHeredity == 3 )
                    patient.HypertensionAncestralAnamnesis = HypertensionAncestralAnamnesis.BothMotherAndFather;
                else if ( hHeredity == 2 )
                    patient.HypertensionAncestralAnamnesis = HypertensionAncestralAnamnesis.Father;
                else if ( hHeredity == 1 )
                    patient.HypertensionAncestralAnamnesis = HypertensionAncestralAnamnesis.Mother;
                else
                    patient.HypertensionAncestralAnamnesis = HypertensionAncestralAnamnesis.None;
            }
            var nameParts = patientProperties["name"].Split( ' ' );
            patient.Name = nameParts[1];
            patient.Surname = nameParts[0];
            patient.MiddleName = nameParts[2];

            patient.BirthDate = DateTime.Today - TimeSpan.FromDays( Convert.ToInt32( patientProperties["age"] ) * 365 );

            patient.Gender = patientProperties["gender"].Contains( "ж" ) ? GenderType.Female : GenderType.Male;
            patient.Genes = new List<Gene>();
            if ( !string.IsNullOrEmpty( patientProperties["AGT"] ) )
                patient.Genes.Add( new Gene
                {
                    Name = "AGT",
                    Value = Convert.ToInt32( patientProperties["AGT"] )
                } );
            if ( !string.IsNullOrEmpty( patientProperties["AGTR2"] ) )
                patient.Genes.Add( new Gene
                {
                    Name = "AGTR2",
                    Value = Convert.ToInt32( patientProperties["AGTR2"] )
                } );
            if ( !string.IsNullOrEmpty( patientProperties["MaleHeredity"] ) )
                patient.MaleHeredity = Convert.ToBoolean( Convert.ToInt32( patientProperties["MaleHeredity"] ) );
            if ( !string.IsNullOrEmpty( patientProperties["FemaleHeredity"] ) )
                patient.FemaleHeredity = Convert.ToBoolean( Convert.ToInt32( patientProperties["FemaleHeredity"] ) );

            if ( patientProperties["smoke"].Contains( "1" ) )
                patientVisitData.Smoking.Type = SmokingType.Never;
            else if ( patientProperties["smoke"].Contains( "2" ) )
                patientVisitData.Smoking.Type = SmokingType.InPast;
            else
                patientVisitData.Smoking.Type = SmokingType.Now;
            if ( patientProperties["HStage"].Contains( "1" ) )
                patientVisitData.HypertensionStage = HypertensionStage.Stage1;
            if ( patientProperties["HStage"].Contains( "2" ) )
                patientVisitData.HypertensionStage = HypertensionStage.Stage2;
            if ( patientProperties["HStage"].Contains( "3" ) )
                patientVisitData.HypertensionStage = HypertensionStage.Stage3;
            if ( patientProperties["HStage"].Contains( "зд" ) )
                patientVisitData.HypertensionStage = HypertensionStage.Healthy;

            if ( !string.IsNullOrEmpty( patientProperties["WaistCircumference"] ) )
                patientVisitData.WaistCircumference = Convert.ToDouble( patientProperties["WaistCircumference"], ruCulture );
            if ( !string.IsNullOrEmpty( patientProperties["BMI"] ) )
                patientVisitData.TemporaryBMI = Convert.ToDouble( patientProperties["BMI"], ruCulture );
            if ( patientProperties["HStage"].Contains( "1" ) )
                patientVisitData.HypertensionStage = HypertensionStage.Stage1;
            else if ( patientProperties["HStage"].Contains( "2" ) )
                patientVisitData.HypertensionStage = HypertensionStage.Stage2;
            else if ( patientProperties["HStage"].Contains( "3" ) )
                patientVisitData.HypertensionStage = HypertensionStage.Stage3;
            else
                patientVisitData.HypertensionStage = HypertensionStage.Healthy;

            if ( !string.IsNullOrEmpty( patientProperties["phiz"] ) )
            {
                var phiz = Convert.ToInt32( patientProperties["phiz"] );
                if ( phiz == 4 )
                    patientVisitData.PhysicalActivity = PhysicalActivity.MoreThenTreeTimesPerWeek;
                else if ( phiz == 3 )
                    patientVisitData.PhysicalActivity = PhysicalActivity.FromOneToThreeTimesPerWeek;
                else if ( phiz == 2 )
                    patientVisitData.PhysicalActivity = PhysicalActivity.OncePerWeekOrLess;
                else
                    patientVisitData.PhysicalActivity = PhysicalActivity.Never;
            }
            return patient;
        }

        #endregion
    }
}
