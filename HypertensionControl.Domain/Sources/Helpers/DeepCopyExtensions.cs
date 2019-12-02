using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HypertensionControl.Domain.Helpers
{
    public static class DeepCopyExtensions
    {
        #region Constants

        private static readonly JsonSerializerSettings DeepCopySerializerSettings =
            new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                NullValueHandling = NullValueHandling.Include,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                ContractResolver = new WritablePropertiesOnlyResolver()
            };

        #endregion


        #region Public methods

        public static TObject DeepCopy<TObject>( this TObject @object )
        {
            var serialized = JsonConvert.SerializeObject( @object, DeepCopySerializerSettings );
            return JsonConvert.DeserializeObject<TObject>( serialized );
        }

        #endregion
    }

    internal class WritablePropertiesOnlyResolver : DefaultContractResolver
    {
        #region Non-public methods

        protected override IList<JsonProperty> CreateProperties( Type type, MemberSerialization memberSerialization )
        {
            var props = base.CreateProperties( type, memberSerialization );
            return props.Where( p => p.Writable ).ToList();
        }

        #endregion
    }
}
