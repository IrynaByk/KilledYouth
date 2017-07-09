using System.Collections.Generic;
using System.Linq;
using System.Text;
using HypertensionControl.Domain.Interfaces;
using HypertensionControl.Domain.Models.Values;

namespace HypertensionControl.Domain.Models
{
    public class RangedListScaler : IScaler
    {
        #region Fields

        private readonly List<Range> _entries;

        #endregion


        #region Properties

        public double this[ double position ]
        {
            get
            {
                //  Default value for the leftmost range
                if ( _entries.Count == 0 || position < _entries[0].LowerBound )
                {
                    return 0;
                }

                //  Search the suitable range
                double result = 0;
                foreach ( var rangeEntry in _entries )
                {
                    if ( rangeEntry.LowerBound > position )
                    {
                        return result;
                    }
                    result = rangeEntry.Value;
                }
                return result;
            }
        }

        #endregion


        #region Initialization

        public RangedListScaler( IEnumerable<Range> entries )
        {
            _entries = entries.OrderBy( rangeEntry => rangeEntry.LowerBound ).ToList();
        }

        #endregion


        #region Public methods

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append( "{ 0" );
            foreach ( var entry in _entries )
            {
                sb.Append( " [" ).Append( entry.LowerBound ).Append( "] " ).Append( entry.Value );
            }
            sb.Append( " }" );
            return sb.ToString();
        }

        #endregion
    }
}
