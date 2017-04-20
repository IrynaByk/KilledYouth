using System.Collections.Generic;

namespace HypertensionControlUI.Utils
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrDefault<TKey, TValue>( this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue) )
        {
            TValue value;
            return dictionary.TryGetValue( key, out value ) ? value : defaultValue;
        }
    }
}