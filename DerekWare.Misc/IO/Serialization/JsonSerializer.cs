using System.IO;
using System.Runtime.Serialization.Json;

namespace DerekWare.IO.Serialization
{
    public abstract class JsonSerializer
    {
        public static readonly DataContractJsonSerializerSettings DefaultSettings = new() { UseSimpleDictionaryFormat = true };
    }

    public class JsonSerializer<T> : JsonSerializer, IStreamSerializer<T>, IStreamDeserializer<T>
    {
        public static readonly JsonSerializer<T> Default = new();

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
}
