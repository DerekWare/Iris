using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace DerekWare.IO.Serialization
{
    public class DataContractSerializer<T> : IStreamSerializer<T>, IStreamDeserializer<T>
    {
        public static readonly DataContractSerializer<T> Default = new DataContractSerializer<T>();

        readonly DataContractSerializer Serializer = new DataContractSerializer(typeof(T));

        #region IStreamDeserializer<T>

        public T Deserialize(Stream stream)
        {
            return (T)Serializer.ReadObject(stream);
        }

        #endregion

        #region IStreamSerializer<T>

        public void Serialize(T obj, Stream stream)
        {
            using(var output = new StringWriter())
            using(var writer = new XmlTextWriter(output))
            {
                Serializer.WriteObject(writer, obj);
            }

            stream.Flush();
        }

        #endregion
    }
}
