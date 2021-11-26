using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using DerekWare.Diagnostics;

namespace DerekWare.HomeAutomation.Common
{
    // This class helps work around some limitations in the XmlSerializer (can't serialize
    // TimeSpan, IDictionary, etc.) as well as application settings (can't add keys at
    // runtime) by using reflection to create and serialize a list of browsable, writable
    // properties used by Effects and Themes.
    public class PropertyCache : Dictionary<Type, PropertyBag>, IPropertyStore
    {
        public static readonly PropertyCache Instance = new();

        protected readonly XmlSerializer Serializer = new(typeof(List<PropertyCacheSerializableItem>));

        #region IPropertyStore

        // Deserializes from XML
        public void Deserialize(string cache)
        {
            try
            {
                using var reader = new StringReader(cache);
                var properties = (List<PropertyCacheSerializableItem>)Serializer.Deserialize(reader);

                foreach(var i in properties)
                {
                    var type = Type.GetType(i.Type, false);

                    if(type is null)
                    {
                        continue;
                    }

                    Add(type, new PropertyBag(i.Properties));
                }
            }
            catch(Exception e)
            {
                Debug.Warning(null, e);
            }
        }

        // Loads all properties from an object into the store
        public void Read(object obj, Type type = null)
        {
            type ??= obj.GetType();
            var propertyBag = new PropertyBag();
            propertyBag.Read(obj, type);
            this[type] = propertyBag;
        }

        // Serializes to XML
        public string Serialize()
        {
            using var writer = new StringWriter();
            var cache = this.Select(i => new PropertyCacheSerializableItem(i.Key, i.Value)).ToList();
            Serializer.Serialize(writer, cache);
            return writer.ToString();
        }

        public IEnumerable ToSerializableTypes()
        {
            return this.Select(i => new PropertyCacheSerializableItem(i.Key, i.Value));
        }

        // Writes all stored properties to an object
        public void Write(object obj, Type type = null)
        {
            type ??= obj.GetType();

            if(!TryGetValue(type, out var propertyBag))
            {
                return;
            }

            propertyBag.Write(obj);
        }

        #endregion
    }

    public class PropertyCacheSerializableItem
    {
        public List<PropertyBagSerializableItem> Properties;
        public string Type;

        public PropertyCacheSerializableItem()
        {
        }

        public PropertyCacheSerializableItem(Type type, PropertyBag properties)
        {
            Type = type.AssemblyQualifiedName;
            Properties = properties.ToSerializableTypes().Cast<PropertyBagSerializableItem>().ToList();
        }
    }
}
