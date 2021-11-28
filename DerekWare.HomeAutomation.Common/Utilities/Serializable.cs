using System;
using System.Runtime.Serialization;

namespace DerekWare.HomeAutomation.Common
{
    // Helper extensions for objects that implement ISerializable. This serializes
    // only the properties found by GetWritableProperties.
    public static class Serializable
    {
        public static void Deserialize(this object @this, SerializationInfo info, StreamingContext context, Type type = null)
        {
            foreach(var item in info)
            {
                @this.SetPropertyValue(item.Name, item.Value, type);
            }
        }

        public static void Serialize<T>(this T @this, SerializationInfo info, StreamingContext context, Type type = null)
        {
            foreach(var property in @this.GetWritableProperties())
            {
                info.AddValue(property.Name, property.GetValue(@this), type);
            }
        }
    }
}
