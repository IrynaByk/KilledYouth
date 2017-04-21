﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Documents;
using HypertensionControlUI.Models;

namespace HypertensionControlUI.Utils
{
    public static class PatientParser
    {
        public static Patient ReadPatientFromDictionary(IDictionary<string, string> patientProperties)
        {
            var patient = new Patient();
            PatientVisitData patientVisitData = new PatientVisitData {Patient = patient};
            patient.PatientVisitDataHistory.Add(patientVisitData);

            var nameParts = patientProperties["name"].Split(' ');
            patient.Name = nameParts[0];
            patient.Surname = nameParts[1];
            patient.MiddleName = nameParts[2];

            patient.BirthDate = DateTime.Today - TimeSpan.FromDays(Convert.ToInt32(patientProperties["age"])*365);

            patient.Gender = patientProperties["gender"].Contains("ж") ? GenderType.Female : GenderType.Male;
            if (!String.IsNullOrEmpty(patientProperties["AGT"]))
            {
                patient.Genes.Add(new Gene { Name = "AGT",
                    Value = Convert.ToInt32(patientProperties["AGT"])});
            }
            if (!String.IsNullOrEmpty(patientProperties["AGTR2"]))
            {
                patient.Genes.Add(new Gene
                {
                    Name = "AGTR2",
                    Value = Convert.ToInt32(patientProperties["AGTR2"])
                });
            }
            if (!String.IsNullOrEmpty(patientProperties["MaleHeredity"]))
            {
                patient.MaleHeredity = Convert.ToBoolean(Convert.ToInt32(patientProperties["MaleHeredity"]));
            }
            if (!String.IsNullOrEmpty(patientProperties["FemaleHeredity"]))
            {
                patient.FemaleHeredity = Convert.ToBoolean(Convert.ToInt32(patientProperties["FemaleHeredity"]));
            }

            if (patientProperties["smoke"].Contains("1"))
            {
                patientVisitData.Smoking.Type = SmokingType.Never;
            }
            else if (patientProperties["smoke"].Contains("2"))
            {
                patientVisitData.Smoking.Type = SmokingType.InPast;
            }
            else
            {
                patientVisitData.Smoking.Type = SmokingType.Now;
            }

            var ruCulture = new CultureInfo("RU-ru");

            if (!String.IsNullOrEmpty(patientProperties["WaistCircumference"]))
            {
                patientVisitData.WaistCircumference = Convert.ToDouble(patientProperties["WaistCircumference"], ruCulture);
            }
            if (!String.IsNullOrEmpty(patientProperties["BMI"]))
            {
                patientVisitData.TemporaryBMI = Convert.ToDouble(patientProperties["BMI"], ruCulture);
            }
            if (patientProperties["HStage"].Contains("1"))
            {
                patientVisitData.HypertensionStage = HypertensionStage.Stage1;

            }
            else if (patientProperties["HStage"].Contains("2"))
            {
                patientVisitData.HypertensionStage = HypertensionStage.Stage2;

            }
            else if (patientProperties["HStage"].Contains("3"))
            {
                patientVisitData.HypertensionStage = HypertensionStage.Stage3;

            }
            else
            {
                patientVisitData.HypertensionStage = HypertensionStage.Healthy;

            }
            return patient;
        }
    }
}