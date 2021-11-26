using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using DerekWare.Collections;
using DerekWare.Reflection;

namespace DerekWare.HomeAutomation.Common
{
    public interface IPropertyStore
    {
        // Deserializes properties from XML
        void Deserialize(string cache);

        // Loads all visible, writable properties from a given object into the dictionary
        void Read(object obj, Type type = null);

        // Serializes the properties to XML
        string Serialize();

        // Converts runtime data to something that can be serialized to XML. This is to
        // work around all the type restrictions used by the XML serializers.
        IEnumerable ToSerializableTypes();

        // Applies all properties to the given object. The reflection extension
        // SetPropertyValue will attempt to convert to the correct type used by
        // the object.
        void Write(object obj, Type type = null);
    }

    public class PropertyBag : Dictionary<string, string>, IPropertyStore
    {
        protected static readonly XmlSerializer Serializer = new(typeof(List<PropertyBagSerializableItem>));

        public PropertyBag()
        {
        }

        public PropertyBag(IEnumerable<KeyValuePair<string, string>> other)
        {
            AddRange(other);
        }

        public PropertyBag(IEnumerable<KeyValuePair<string, object>> other)
        {
            AddRange(other);
        }

        public PropertyBag(IEnumerable<PropertyBagSerializableItem> other)
        {
            AddRange(other);
        }

        public new void Add(string key, string value)
        {
            base.Add(key, value);
        }

        public void Add(string key, object value)
        {
            base.Add(key, value.Convert<string>());
        }

        public void AddRange(IEnumerable<KeyValuePair<string, string>> items)
        {
            items.SafeEmpty().ForEach(i => Add(i.Key, i.Value));
        }

        public void AddRange(IEnumerable<KeyValuePair<string, object>> items)
        {
            items.SafeEmpty().ForEach(i => Add(i.Key, i.Value));
        }

        public void AddRange(IEnumerable<PropertyBagSerializableItem> items)
        {
            items.SafeEmpty().ForEach(i => Add(i.Key, i.Value));
        }

        #region IPropertyStore

        // Deserializes properties from XML
        public void Deserialize(string cache)
        {
            using var reader = new StringReader(cache);
            AddRange((List<PropertyBagSerializableItem>)Serializer.Deserialize(reader));
        }

        // Loads all visible, writable properties from a given object into the dictionary
        public void Read(object obj, Type type = null)
        {
            type ??= obj.GetType();
            AddRange(type.GetWritableProperties().Select(i => new KeyValuePair<string, object>(i.Name, i.GetValue(obj))));
        }

        // Serializes the properties to XML
        public string Serialize()
        {
            using var writer = new StringWriter();
            Serializer.Serialize(writer, ToSerializableTypes());
            return writer.ToString();
        }

        // Converts all properties to a serializable type
        public IEnumerable ToSerializableTypes()
        {
            return this.Select(i => new PropertyBagSerializableItem(i.Key, i.Value));
        }

        // Applies all properties to the given object. The reflection extension
        // SetPropertyValue will attempt to convert to the correct type used by
        // the object.
        public void Write(object obj, Type type = null)
        {
            this.ForEach(i => obj.SetPropertyValue(i.Key, i.Value, type));
        }

        #endregion
    }

    public class PropertyBagSerializableItem
    {
        public string Key;
        public string Value;

        public PropertyBagSerializableItem()
        {
        }

        public PropertyBagSerializableItem(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
