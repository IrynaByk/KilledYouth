using System;

namespace HypertensionControl.Domain.Services
{
    public static class PatientPropertyProvider
    {
        #region Public methods

        public static object GetPropertyValue( object propertySource, string propertyName )
        {
            var currentObject = propertySource;

            //  Traverse the property names chain
            foreach ( var propertyNamePart in propertyName.Split( '.' ) )
            {
                var type = currentObject.GetType();
                var propertyInfo = type.GetProperty( propertyNamePart );
                if ( propertyInfo == null )
                {
                    throw new InvalidOperationException( $"Property '{propertyNamePart}' not found" );
                }

                currentObject = propertyInfo.GetValue( currentObject );
            }

            return currentObject;
        }

        public static void UpdatePatientByProperty( string propertyName, object source, object value )
        {
            var currentObject = source; //CurrentObject( ref propertyName, patient, visitData );
            var pathStrings = propertyName.Split( '.' );
            for ( var i = 0; i < pathStrings.Length - 1; i++ )
            {
                var propertyNamePart = pathStrings[i];
                var type = currentObject.GetType();
                var propertyInfo = type.GetProperty( propertyNamePart );
                if ( propertyInfo == null )
                {
                    throw new InvalidOperationException( $"Property '{propertyNamePart}' not found" );
                }

                currentObject = propertyInfo.GetValue( currentObject );
            }

            var propertyInfoLast = currentObject.GetType().GetProperty( pathStrings[pathStrings.Length - 1] );
            if ( propertyInfoLast == null )
            {
                throw new InvalidOperationException( $"Property '{propertyName}' not found" );
            }
            propertyInfoLast.SetValue( currentObject, Convert( value, propertyInfoLast.PropertyType ) );
        }

        #endregion


        #region Non-public methods

        private static object Convert( object value, Type targetType )
        {
            //  Check for Enum or Enum? type
            if ( targetType.IsEnum )
            {
                return Enum.Parse( targetType, value.ToString() );
            }
            if ( Nullable.GetUnderlyingType( targetType ) is Type underlyingType && underlyingType.IsEnum )
            {
                return Enum.Parse( underlyingType, value.ToString() );
            }

            //  
            try
            {
                return System.Convert.ChangeType( value, targetType );
            }
            catch ( Exception ex ) when ( ex is FormatException || ex is InvalidCastException || ex is ArgumentNullException )
            {
                return targetType.IsValueType ? Activator.CreateInstance( targetType ) : null;
            }
        }

        #endregion
    }
}
