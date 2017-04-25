using System.Collections.Generic;
using System.Linq;
using HypertensionControlUI.Models;

namespace HypertensionControlUI.Services
{
    public class OptimalCutOffCalculator
    {
        private readonly PatientPropertyProvider _propertyProvider;

        public OptimalCutOffCalculator(PatientPropertyProvider propertyProvider)
        {
            _propertyProvider = propertyProvider;
        }

        public double CalculateOptimalCutOff(ClassificationModel model, List<Patient> patients)
        {
            var classificator = new PatientClassificator(model, _propertyProvider);

            var healthyCorrect = 0;
            var illCorrect = patients.Count(p => p.Stage != HypertensionStage.Healthy);
            var healthyIncorrect = patients.Count(p => p.Stage == HypertensionStage.Healthy);
            var illIncorrect = 0;

            var basePatientStageScoresGroupped =
                patients.Select(patient => new
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
            foreach (var cutoff in cutoffs)
            {
                
            }

            return 0;
        }

        private struct CutOffValues
        {
            public double Score { get; set; }
            public int HealthyCorrect { get; set; }
            public int HealthyIncorrect { get; set; }
            public int IllCorrect { get; set; }
            public int IllIncorrect { get; set; }
        }
    }
}