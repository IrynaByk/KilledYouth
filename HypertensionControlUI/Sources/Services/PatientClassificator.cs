using System;
using System.Collections.Generic;
using System.Linq;
using HypertensionControlUI.Models;
using HypertensionControlUI.Models.Runtime;

namespace HypertensionControlUI.Services
{
    public class PatientClassificator
    {
        #region Auto-properties

        public RuntimeClassificationModel RuntimeClassificationModel { get; set; }

        #endregion


        #region Initialization

        public PatientClassificator(ClassificationModel classificationModel)
        {
            RuntimeClassificationModel = new RuntimeClassificationModel( classificationModel );
        }

        #endregion


        #region Public methods

        public double Classify( Patient patient, PatientVisitData visitData )
        {
            var enumerable = RuntimeClassificationModel
                .Properties
                .Select(p => p.Scaler[Convert.ToDouble(PatientPropertyProvider.GetPropertyValue(p.Name, patient, visitData))] * p.ModelCoefficient)
                .ToList();

            var intermediateResult = enumerable.Sum() + RuntimeClassificationModel.FreeCoefficient;

            return Math.Exp( intermediateResult )/(1 + Math.Exp( intermediateResult ));
        }

        
        #endregion
    }
}
