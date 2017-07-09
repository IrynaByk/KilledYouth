using System;
using System.Linq;
using HypertensionControl.Domain.Models;

namespace HypertensionControl.Domain.Services
{
    public class PatientClassificator
    {
        #region Auto-properties

        public ClassificationModel ClassificationModel { get; set; }

        #endregion


        #region Initialization

        public PatientClassificator( ClassificationModel classificationModel )
        {
            ClassificationModel = classificationModel;
        }

        #endregion


        #region Public methods

        public double Classify( object dataSource )
        {
            var enumerable = ClassificationModel
                .Properties
                .Select( p => p.Scaler[Convert.ToDouble( PatientPropertyProvider.GetPropertyValue( dataSource, p.Name ) )] * p.Coefficient )
                .ToList();

            var intermediateResult = enumerable.Sum() + ClassificationModel.FreeCoefficient;

            return Math.Exp( intermediateResult ) / (1 + Math.Exp( intermediateResult ));
        }

        #endregion
    }   
}
