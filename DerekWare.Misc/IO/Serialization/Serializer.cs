using System;
using System.IO;
using DerekWare.Diagnostics;

namespace DerekWare.IO.Serialization
{
    public interface IStreamDeserializer<out T>
    {
        T Deserialize(Stream stream);
    }

    public interface IStreamSerializer<in T>
    {
        void Serialize(T obj, Stream stream);
    }

    public static class Serializer
    {
        public static T DeserializeFile<T>(
            this IStreamDeserializer<T> serializer,
            Path fileName,
            FileMode fileMode = FileMode.Open,
            FileAccess fileAccess = FileAccess.Read)
        {
            using(var stream = new FileStream(fileName, fileMode, fileAccess))
            {
                return serializer.Deserialize(stream);
            }
        }

        public static T DeserializeString<T>(this IStreamDeserializer<T> serializer, string value)
        {
            using(var stream = new MemoryStream())
            using(var writer = new StreamWriter(stream))
            {
                writer.Write(value);
                writer.Flush();
                stream.Position = 0;
                return serializer.Deserialize(stream);
            }
        }

        public static void SerializeFile<T>(
            this IStreamSerializer<T> serializer,
            T obj,
            Path fileName,
            FileMode fileMode = FileMode.Create,
            FileAccess fileAccess = FileAccess.Write)
        {
            using(var stream = new FileStream(fileName, fileMode, fileAccess))
            {
                serializer.Serialize(obj, stream);
            }
        }

        public static string SerializeString<T>(this IStreamSerializer<T> serializer, T obj)
        {
            using(var stream = new MemoryStream())
            {
                serializer.Serialize(obj, stream);
                stream.Position = 0;

                using(var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static bool TryDeserialize<T>(this IStreamDeserializer<T> serializer, Stream stream, out T obj)
        {
            try
            {
                obj = serializer.Deserialize(stream);
                return true;
            }
            catch(Exception ex)
            {
                Debug.Warning(serializer, ex);
                obj = default;
                return false;
            }
        }

        public static bool TryDeserializeFile<T>(this IStreamDeserializer<T> serializer, Path fileName, out T obj)
        {
            return TryDeserializeFile(serializer, fileName, FileMode.Open, FileAccess.Read, out obj);
        }

        public static bool TryDeserializeFile<T>(this IStreamDeserializer<T> serializer, Path fileName, FileMode fileMode, FileAccess fileAccess, out T obj)
        {
            try
            {
                obj = DeserializeFile(serializer, fileName, fileMode, fileAccess);
                return true;
            }
            catch(Exception ex)
            {
                Debug.Warning(serializer, ex);
                obj = default;
                return false;
            }
        }

        public static bool TryDeserializeString<T>(this IStreamDeserializer<T> serializer, string value, out T obj)
        {
            try
            {
                obj = DeserializeString(serializer, value);
                return true;
            }
            catch(Exception ex)
            {
                Debug.Warning(serializer, ex);
                obj = default;
                return false;
            }
        }
    }
}
