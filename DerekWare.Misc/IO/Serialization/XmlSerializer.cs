using System.IO;
using System.Xml;

namespace DerekWare.IO.Serialization
{
    public abstract class XmlSerializer
    {
        public static readonly XmlWriterSettings DefaultSettings = new() { Indent = true, IndentChars = "\t" };
    }

    public class XmlSerializer<T> : XmlSerializer, IStreamSerializer<T>, IStreamDeserializer<T>
    {
        public static readonly XmlSerializer<T> Default = new();

        readonly System.Xml.Serialization.XmlSerializer Serializer = new(typeof(T));
        readonly XmlWriterSettings Settings;

        public XmlSerializer()
            : this(null)
        {
        }

        public XmlSerializer(XmlWriterSettings settings)
        {
            Settings = settings ?? DefaultSettings;
        }

        #region IStreamDeserializer<T>

        public T Deserialize(Stream stream)
        {
            return (T)Serializer.Deserialize(stream);
        }

        #endregion

        #region IStreamSerializer<T>

        public void Serialize(T obj, Stream stream)
        {
            using(var writer = XmlWriter.Create(stream, Settings))
            {
                Serializer.Serialize(writer, obj);
            }

            stream.Flush();
        }

        #endregion
    }
}
