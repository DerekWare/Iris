using System;
using DerekWare.Collections;
using DerekWare.Diagnostics;
using Newtonsoft.Json;

namespace DerekWare.HomeAutomation.Common
{
    // This class helps work around some limitations in the XmlSerializer (can't serialize
    // TimeSpan, IDictionary, etc.) as well as application settings (can't add keys at
    // runtime) by using reflection to create and serialize a list of browsable, writable
    // properties used by Effects and Themes.
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
            AddRange(JsonConvert.DeserializeObject<PropertyCache>(cache));
        }

        public string Serialize()
        {
            var cache = JsonConvert.SerializeObject(this);
            Debug.Trace(this, cache);
            return cache;
        }

        #endregion
    }
}
