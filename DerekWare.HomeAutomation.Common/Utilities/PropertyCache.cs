using System;
using DerekWare.Collections;
using DerekWare.Diagnostics;
using Newtonsoft.Json;

namespace DerekWare.HomeAutomation.Common
{
    // PropertyCache uses reflection to read and write properties for a given type. This
    // is useful because the JSON serializer doesn't do well with serializing 'object', so
    // we use reflection to pull all the browsable, writable properties from any object
    // and store them in the PropertyBag for serializing. A typical pattern for using this
    // is to call WriteToObject, then display a PropertyGrid dialog to let the user change
    // any properties, then call ReadFromObject to save those changes as the new defaults.
    // PropertyCache and PropertyBag don't implement ISerializable because it's not necessary
    // and doesn't even work with dictionaries in Newtonsoft JSON.
    [Serializable, JsonObject]
    public class PropertyCache : ObservableDictionary<string, PropertyBag>, ISerializablePropertyStore<string, PropertyBag>
    {
        #region IPropertyStore<string,PropertyBag>

        public void ReadFromObject(object obj, Type type = null)
        {
            type ??= obj.GetType();
            var propertyBag = new PropertyBag();
            propertyBag.ReadFromObject(obj, type);
            this[type.AssemblyQualifiedName] = propertyBag;
        }

        public void WriteToObject(object obj, Type type = null)
        {
            type ??= obj.GetType();

            if(!TryGetValue(type.AssemblyQualifiedName, out var propertyBag))
            {
                return;
            }

            propertyBag.WriteToObject(obj);
        }

        #endregion

        #region ISerializablePropertyStore<string,PropertyBag>

        public void Deserialize(string cache)
        {
            Debug.Trace(this, cache);
            AddRange(JsonSerializer.Deserialize<PropertyCache>(cache));
        }

        public string Serialize()
        {
            var cache = JsonSerializer.Serialize(this);
            Debug.Trace(this, cache);
            return cache;
        }

        #endregion
    }
}
