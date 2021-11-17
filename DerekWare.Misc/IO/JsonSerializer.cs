using System;
using System.IO;
using System.Runtime.Serialization.Json;
using DerekWare.Diagnostics;

namespace DerekWare.IO
{
    public class JsonSerializer<T> : IStreamSerializer<T>, IStreamDeserializer<T>
    {
        public static readonly JsonSerializer<T> Default = new JsonSerializer<T>();
        public static readonly DataContractJsonSerializerSettings DefaultSettings = new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true };

        readonly DataContractJsonSerializer Serializer;

        public JsonSerializer()
            : this(null)
        {
        }

        public JsonSerializer(DataContractJsonSerializerSettings settings)
        {
            Serializer = new DataContractJsonSerializer(typeof(T), settings ?? DefaultSettings);
        }

        #region IStreamDeserializer<T>

        public T Deserialize(Stream stream)
        {
            return (T)Serializer.ReadObject(stream);
        }

        #endregion

        #region IStreamSerializer<T>

        public void Serialize(T obj, Stream stream)
        {
            Serializer.WriteObject(stream, obj);
            stream.Flush();
        }

        #endregion
    }

    public static partial class Serializer
    {
        public static string SerializeJson<T>(this T @this)
        {
            return SerializeString(new JsonSerializer<T>(), @this);
        }

        public static void SerializeJson<T>(this T @this, Path fileName, FileMode fileMode = FileMode.Create, FileAccess fileAccess = FileAccess.Write)
        {
            SerializeFile(new JsonSerializer<T>(), @this, fileName, fileMode, fileAccess);
        }

        public static bool TrySerializeJson<T>(this T @this, out string value)
        {
            try
            {
                value = SerializeJson(@this);
                return true;
            }
            catch(Exception ex)
            {
                Debug.Warning(@this, ex);
                value = null;
                return false;
            }
        }

        public static bool TrySerializeJson<T>(this T @this, Path fileName, FileMode fileMode = FileMode.Create, FileAccess fileAccess = FileAccess.Write)
        {
            try
            {
                SerializeJson(@this, fileName, fileMode, fileAccess);
                return true;
            }
            catch(Exception ex)
            {
                Debug.Warning(@this, ex);
                return false;
            }
        }
    }
}
