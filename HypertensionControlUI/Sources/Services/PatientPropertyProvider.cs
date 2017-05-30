using System;
using HypertensionControlUI.Models;

namespace HypertensionControlUI.Services
{
    public class PatientPropertyProvider
    {
        private const string PatientVisitDataPrefix = "{PatientVisitData}.";

        public double? GetPropertyValue(string propertyName, Patient patient, PatientVisitData visitData)
        {
            object currentObject = CurrentObject(ref propertyName, patient, visitData);

            foreach(var propertyNamePart in propertyName.Split( '.' ))
            {
                var type = currentObject.GetType();
                var propertyInfo = type.GetProperty( propertyNamePart );
                if ( propertyInfo == null )
                    throw new InvalidOperationException( string.Format( "Property '{0}' not found", propertyNamePart ) );

                currentObject = propertyInfo.GetValue( currentObject );
            }

            if ( currentObject == null )
                return null;

            try
            {
                return Convert.ToDouble( currentObject );
            }
            catch (FormatException)
            {
                throw new InvalidOperationException( string.Format( "Property '{0}' has value '{1}' which cannot be converted to double",
                                                                    propertyName, currentObject ) );
            }
        }

        private object CurrentObject(ref string propertyName, Patient patient, PatientVisitData visitData)
        {
            object currentObject;
            if (propertyName.StartsWith(PatientVisitDataPrefix))
            {
                currentObject = visitData;
                propertyName = propertyName.Substring(PatientVisitDataPrefix.Length);
            }
            else
            {
                currentObject = patient;
            }
            return currentObject;
        }

        public void UpdatePatientByProperty(string propertyName, Patient patient, PatientVisitData visitData, object value)
        {
            object currentObject = CurrentObject(ref propertyName, patient, visitData);
            var pathStrings = propertyName.Split('.');
            for (int i = 0; i < pathStrings.Length - 1; i++)
            {
                var propertyNamePart = pathStrings[i];
                var type = currentObject.GetType();
                var propertyInfo = type.GetProperty(propertyNamePart);
                if (propertyInfo == null)
                    throw new InvalidOperationException(string.Format("Property '{0}' not found", propertyNamePart));

                currentObject = propertyInfo.GetValue(currentObject);
            }
           
            var propertyInfoLast = currentObject.GetType().GetProperty(pathStrings[pathStrings.Length - 1]);
            if (propertyInfoLast == null)
                throw new InvalidOperationException(string.Format("Property '{0}' not found", propertyInfoLast));
            propertyInfoLast.SetValue(currentObject.GetType(), value);
        }
    }
}