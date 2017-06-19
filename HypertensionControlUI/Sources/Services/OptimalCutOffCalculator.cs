using System;
using System.Collections.Generic;
using System.Linq;
using HypertensionControlUI.Models;

namespace HypertensionControlUI.Services
{
    public class OptimalCutOffCalculator
    {
        public double CalculateOptimalCutOff(ClassificationModel model, List<Patient> patients)
        {
            //ToDo: filter patients which could be used for current model 
            var applicablePatients = patients.Where(
                                                 patient => model.Properties.All(p =>
                                                     PatientPropertyProvider.GetPropertyValue(
                                                         p.Name, patient, patient.PatientVisitDataHistory.OrderByDescending(d => d.VisitDate).First()) != null))
                                             .ToList();

            foreach (var p in applicablePatients)
            {
                var temp = p.PatientVisitDataHistory.OrderByDescending(d => d.VisitDate).First();
                Console.Write(p.Surname + " bmi "  + temp.ObesityBMI + " waist " +temp.ObesityWaistCircumference + " gene " + p.AGT_AGTR2 + " " + " gender " + p.Gender + " phiz " + temp.PhysicalActivity +  " maleHered "+ p.MaleHeredity + "; \n");
            }
            var classificator = new PatientClassificator(model);

            var healthyCorrect = 0;
            var illCorrect = applicablePatients.Count(p => p.Stage != HypertensionStage.Healthy);
            var healthyIncorrect = applicablePatients.Count(p => p.Stage == HypertensionStage.Healthy);
            var illIncorrect = 0;

            var basePatientStageScoresGroupped =
                applicablePatients.Select(patient => new
                                           {
                                               Patient = patient,
                                               Score = classificator.Classify(patient, patient.PatientVisitDataHistory.OrderByDescending(d => d.VisitDate).First())
                                           })
                        .GroupBy(patientScore => patientScore.Score)
                        .OrderBy(scoreGroup => scoreGroup.Key)
                        .ToList();

            var cutoffs = new List<CutOffValues>
                          {
                              new CutOffValues
                              {
                                  HealthyCorrect = healthyCorrect,
                                  HealthyIncorrect = healthyIncorrect,
                                  IllCorrect = illCorrect,
                                  IllIncorrect = illIncorrect,
                                  Score = 0
                              }
                          };


            foreach (var scoreGroup in basePatientStageScoresGroupped)
            {
                foreach (var patientScore in scoreGroup)
                {
                    if (patientScore.Patient.Stage == HypertensionStage.Healthy)
                    {
                        healthyCorrect ++;
                        healthyIncorrect --;
                    }
                    else
                    {
                        illCorrect--;
                        illIncorrect++;
                    }
                }

                cutoffs.Add(new CutOffValues
                            {
                                HealthyCorrect = healthyCorrect,
                                HealthyIncorrect = healthyIncorrect,
                                IllCorrect = illCorrect,
                                IllIncorrect = illIncorrect,
                                Score = scoreGroup.Key
                            }
                    );
            }
            var cutoff = cutoffs.OrderByDescending(c => c.SumPercent).ThenBy(c => c.PercentDifference);
//            foreach (var coutoffTest in cutoffs)
//            {
//                Console.Write(coutoffTest.SumPercent + " " + coutoffTest.HealthPercent + " " + coutoffTest.IllPercent + "; \n");
//            }
            var cutoff2 = cutoffs.OrderBy(c => c.PercentDifference).ThenByDescending(c => c.SumPercent);

            return cutoff.FirstOrDefault().Score;
        }

        private struct CutOffValues
        {
            public double Score { get; set; }
            public int HealthyCorrect { get; set; }
            public int HealthyIncorrect { get; set; }
            public int IllCorrect { get; set; }
            public int IllIncorrect { get; set; }

            public double HealthPercent => (HealthyCorrect != 0)?(double)HealthyIncorrect / (HealthyCorrect + HealthyIncorrect) : 0.0;
            public double IllPercent => (IllCorrect != 0) ? (double) IllIncorrect / (IllCorrect + IllIncorrect) : 0.0;

            public double SumPercent => ((HealthyCorrect + IllCorrect) != 0) ? (double)(HealthyIncorrect + IllIncorrect) / (HealthyCorrect + IllCorrect + HealthyIncorrect + IllIncorrect) : 0.0;
            public double PercentDifference => (HealthPercent == 0 || IllPercent == 0) ? Double.MaxValue : Math.Abs(HealthPercent - IllPercent);
        }
    }
}