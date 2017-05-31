using System;
using HypertensionControlUI.Models;

namespace HypertensionControlUI.Services
{
    public static class PatientPropertyProvider
    {
        #region Constants

        private const string PatientVisitDataPrefix = "{PatientVisitData}.";

        #endregion


        #region Public methods

        public static object GetPropertyValue( string propertyName, Patient patient, PatientVisitData visitData )
        {
            var currentObject = CurrentObject( ref propertyName, patient, visitData );

            foreach ( var propertyNamePart in propertyName.Split( '.' ) )
            {
                var type = currentObject.GetType();
                var propertyInfo = type.GetProperty( propertyNamePart );
                if ( propertyInfo == null )
                    throw new InvalidOperationException( $"Property '{propertyNamePart}' not found" );

                currentObject = propertyInfo.GetValue( currentObject );
            }

            return currentObject;
        }

        public static void UpdatePatientByProperty( string propertyName, Patient patient, PatientVisitData visitData, object value )
        {
            var currentObject = CurrentObject( ref propertyName, patient, visitData );
            var pathStrings = propertyName.Split( '.' );
            for ( var i = 0; i < pathStrings.Length - 1; i++ )
            {
                var propertyNamePart = pathStrings[i];
                var type = currentObject.GetType();
                var propertyInfo = type.GetProperty( propertyNamePart );
                if ( propertyInfo == null )
                    throw new InvalidOperationException( $"Property '{propertyNamePart}' not found" );

                currentObject = propertyInfo.GetValue( currentObject );
            }

            var propertyInfoLast = currentObject.GetType().GetProperty( pathStrings[pathStrings.Length - 1] );
            if ( propertyInfoLast == null )
                throw new InvalidOperationException( $"Property '{propertyName}' not found" );
            propertyInfoLast.SetValue( currentObject, Convert.ChangeType( value, propertyInfoLast.PropertyType ) );
        }

        #endregion


        #region Non-public methods

        private static object CurrentObject( ref string propertyName, Patient patient, PatientVisitData visitData )
        {
            object currentObject;
            if ( propertyName.StartsWith( PatientVisitDataPrefix ) )
            {
                currentObject = visitData;
                propertyName = propertyName.Substring( PatientVisitDataPrefix.Length );
            }
            else
            {
                currentObject = patient;
            }
            return currentObject;
        }

        #endregion
    }
}
