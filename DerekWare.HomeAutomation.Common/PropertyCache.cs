using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using DerekWare.Collections;
using DerekWare.Diagnostics;

namespace DerekWare.HomeAutomation.Common
{
    // This class helps work around some limitations in the XmlSerializer (can't serialize
    // TimeSpan, IDictionary, etc.) as well as application settings (can't add keys at
    // runtime) by using reflection to create and serialize a list of browsable, writable
    // properties used by Effects and Scenes.
    public static class PropertyCache
    {
        static readonly Dictionary<Type, Dictionary<string, object>> Items = new();
        static readonly XmlSerializer Serializer = new(typeof(List<PropertyList>));

        public static void Load(string cache)
        {
            Items.Clear();

            // Deserialize the string collection as a set of PropertyLists
            try
            {
                using var reader = new StringReader(cache);
                var properties = (List<PropertyList>)Serializer.Deserialize(reader);

                foreach(var i in properties)
                {
                    var type = Type.GetType(i.Type, false);

                    if(type is null)
                    {
                        continue;
                    }

                    Items.Add(type, i.Properties.ToDictionary(entry => entry.Name, entry => entry.Value));
                }
            }
            catch(Exception e)
            {
                Debug.Warning(null, e);
            }
        }

        public static void LoadProperties(object obj)
        {
            var type = obj.GetType();

            if(!Items.TryGetValue(type, out var cache))
            {
                return;
            }

            var properties = Factory.GetWritableProperties(type).Select(i => new KeyValuePair<string, PropertyInfo>(i.Name, i)).ToDictionary();

            foreach(var cacheItem in cache)
            {
                if(!properties.TryGetValue(cacheItem.Key, out var property))
                {
                    continue;
                }

                var propertyValue = cacheItem.Value;

                // Special-case non-serializable types
                if(typeof(TimeSpan) == property.PropertyType)
                {
                    propertyValue = TimeSpan.Parse((string)cacheItem.Value);
                }

                property.SetValue(obj, propertyValue);
            }
        }

        public static string Save()
        {
            var properties = Items.Select(i => new PropertyList
                                  {
                                      Type = i.Key.AssemblyQualifiedName,
                                      Properties = new List<PropertyEntry>(i.Value.Select(j => new PropertyEntry { Name = j.Key, Value = j.Value }))
                                  })
                                  .ToList();

            using var writer = new StringWriter();
            Serializer.Serialize(writer, properties);
            return writer.ToString();
        }

        public static void SaveProperties(object obj)
        {
            var type = obj.GetType();
            var properties = Factory.GetWritableProperties(type);
            var cache = new Dictionary<string, object>();

            // Special-case non-serializable types
            foreach(var property in properties)
            {
                var propertyValue = property.GetValue(obj);

                if(typeof(TimeSpan) == property.PropertyType)
                {
                    propertyValue = ((TimeSpan)propertyValue).ToString();
                }

                cache[property.Name] = propertyValue;
            }

            Items[obj.GetType()] = cache;
        }

        public class PropertyEntry
        {
            public string Name;
            public object Value;
        }

        public class PropertyList
        {
            public List<PropertyEntry> Properties;
            public string Type;
        }
    }
}
