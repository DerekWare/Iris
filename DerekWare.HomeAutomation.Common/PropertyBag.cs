using System;
using System.Collections.Generic;
using System.Linq;
using DerekWare.Collections;

namespace DerekWare.HomeAutomation.Common
{
    public interface IPropertyStore<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        // Reads all visible, writable properties from a given object into the dictionary
        void ReadFromObject(object obj, Type type = null);

        // Applies all stored properties to the given object. The reflection extension
        // SetPropertyValue will attempt to convert to the correct type used by the object.
        void WriteToObject(object obj, Type type = null);
    }

    public interface ISerializablePropertyStore<TKey, TValue> : IPropertyStore<TKey, TValue>
    {
        // Deserializes properties from storage
        void Deserialize(string cache);

        // Serializes the properties to storage
        string Serialize();
    }

    public class PropertyBag : ObservableDictionary<string, object>, IPropertyStore<string, object>
    {
        #region IPropertyStore<string,object>

        // Reads all visible, writable properties from a given object into the dictionary
        public void ReadFromObject(object obj, Type type = null)
        {
            type ??= obj.GetType();
            AddRange(type.GetWritableProperties().Select(i => new KeyValuePair<string, object>(i.Name, i.GetValue(obj))));
        }

        // Applies all stored properties to the given object. The reflection extension
        // SetPropertyValue will attempt to convert to the correct type used by the object.
        public void WriteToObject(object obj, Type type = null)
        {
            this.ForEach(i => obj.SetPropertyValue(i.Key, i.Value, type));
        }

        #endregion
    }
}
