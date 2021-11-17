using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using DerekWare.Diagnostics;

namespace DerekWare.IO
{
    public class XmlSerializer<T> : IStreamSerializer<T>, IStreamDeserializer<T>
    {
        public static readonly XmlSerializer<T> Default = new XmlSerializer<T>();
        public static readonly XmlWriterSettings DefaultSettings = new XmlWriterSettings { Indent = true, IndentChars = "\t" };

        readonly XmlSerializer Serializer = new XmlSerializer(typeof(T));
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

    public static partial class Serializer
    {
        public static string SerializeXml<T>(this T @this)
        {
            return SerializeString(new XmlSerializer<T>(), @this);
        }

        public static void SerializeXml<T>(this T @this, Path fileName, FileMode fileMode = FileMode.Create, FileAccess fileAccess = FileAccess.Write)
        {
            SerializeFile(new XmlSerializer<T>(), @this, fileName, fileMode, fileAccess);
        }

        public static bool TrySerializeXml<T>(this T @this, out string value)
        {
            try
            {
                value = SerializeXml(@this);
                return true;
            }
            catch(Exception ex)
            {
                Debug.Warning(@this, ex);
                value = null;
                return false;
            }
        }

        public static bool TrySerializeXml<T>(this T @this, Path fileName, FileMode fileMode = FileMode.Create, FileAccess fileAccess = FileAccess.Write)
        {
            try
            {
                SerializeXml(@this, fileName, fileMode, fileAccess);
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
