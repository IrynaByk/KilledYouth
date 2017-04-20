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

        public PatientPropertyProvider PatientPropertyProvider { get; set; }
        public RuntimeClassificationModel RuntimeClassificationModel { get; set; }

        #endregion


        #region Initialization

        public PatientClassificator( ClassificationModel classificationModel, PatientPropertyProvider patientPropertyProvider )
        {
            RuntimeClassificationModel = new RuntimeClassificationModel( classificationModel );
            PatientPropertyProvider = patientPropertyProvider;
        }

        #endregion


        #region Public methods

        public double Classify( Patient patient, PatientVisitData visitData )
        {
            var enumerable = RuntimeClassificationModel
                .Properties
                .Select( p => p.Scaler[(double) PatientPropertyProvider.GetPropertyValue( p.Name, patient, visitData )]*p.ModelCoefficient )
                .ToList();

            var intermediateResult = enumerable.Sum() + RuntimeClassificationModel.FreeCoefficient;

            return Math.Exp( intermediateResult )/(1 + Math.Exp( intermediateResult ));
        }

        
        #endregion
    }
}
