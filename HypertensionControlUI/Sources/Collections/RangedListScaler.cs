using System.Collections.Generic;
using System.Linq;

namespace HypertensionControlUI.Collections
{
    public class RangedListScaler : IScaler
    {
        #region Fields

        private readonly List<RangeEntry> _entries;

        #endregion


        #region Properties

        public double this[ double position ]
        {
            get
            {
                if ( _entries.Count == 0 || (position < _entries[0].LowerBound) )
                    return 0;
                double result = 0;
                foreach ( var rangeEntry in _entries )
                {
                    if ( rangeEntry.LowerBound > position )
                        return result;
                    result = rangeEntry.Value;
                }
                return result;
            }
        }

        #endregion


        #region Initialization

        public RangedListScaler( IEnumerable<RangeEntry> entries )
        {
            _entries = entries.OrderBy( rangeEntry => rangeEntry.LowerBound ).ToList();
        }

        #endregion
    }
}
