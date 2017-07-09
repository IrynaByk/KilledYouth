using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using HypertensionControl.Domain.Interfaces;

namespace HypertensionControlUI.Services
{
    public class ResourceProvider : IResourceProvider
    {
        #region Public methods

        public Stream GetResourceStream( string name )
        {
            return Application.GetResourceStream( new Uri( $"/Resources/{name}", UriKind.Relative ) )?.Stream;
        }

        public string[] ReadAllResourceLines( string name )
        {
            var stringList = new List<string>();

            using ( var stream = GetResourceStream( name ) )
            using ( var streamReader = new StreamReader( stream ) )
            {
                string str;
                while ( (str = streamReader.ReadLine()) != null )
                    stringList.Add( str );
            }

            return stringList.ToArray();
        }

        public string ReadAllResourceText( string name )
        {
            using ( var stream = GetResourceStream( name ) )
            using ( var streamReader = new StreamReader( stream ) )
            {
                return streamReader.ReadToEnd();
            }
        }

        #endregion
    }
}
