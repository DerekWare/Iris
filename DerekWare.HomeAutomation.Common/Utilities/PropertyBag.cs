using System;
using System.Collections.Generic;
using System.Linq;
using DerekWare.Collections;
using Newtonsoft.Json;

namespace DerekWare.HomeAutomation.Common
{
    public interface IPropertyStore<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        /// <summary>
        ///     Reads all visible, writable properties from a given object into the dictionary
        /// </summary>
        /// <param name="obj">The source object</param>
        /// <param name="type">The base type to use when loading properties or null to use the object type.</param>
        void ReadFromObject(object obj, Type type = null);

        /// <summary>
        ///     Applies all stored properties to the given object. The reflection extension
        ///     SetPropertyValue will attempt to convert to the correct type used by the object.
        /// </summary>
        /// <param name="obj">The target object</param>
        /// <param name="type">The base type to use when loading properties or null to use the object type.</param>
        void WriteToObject(object obj, Type type = null);
    }

    public interface ISerializablePropertyStore<TKey, TValue> : IPropertyStore<TKey, TValue>
    {
        void Deserialize(string cache);
        string Serialize();
    }

    [Serializable, JsonObject]
    public class PropertyBag : ObservableDictionary<string, object>, IPropertyStore<string, object>
    {
        public PropertyBag()
        {
        }

        public PropertyBag(object obj, Type type = null)
        {
            ReadFromObject(obj, type);
        }

        #region IPropertyStore<string,object>

        public void ReadFromObject(object obj, Type type = null)
        {
            type ??= obj.GetType();
            AddRange(type.GetWritableProperties().Select(i => new KeyValuePair<string, object>(i.Name, i.GetValue(obj))));
        }

        public void WriteToObject(object obj, Type type = null)
        {
            this.ForEach(i => obj.SetPropertyValue(i.Key, i.Value, type));
        }

        #endregion
    }
}
