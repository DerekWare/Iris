using System;
using DerekWare.Collections;
using DerekWare.IO;
using DerekWare.Net.RemoteFileSystem.Local;

namespace DerekWare.Net.RemoteFileSystem
{
    public static class Activator
    {
        public static object CreateInstance(Type type, Path path)
        {
            if(path.Scheme.IsNullOrEmpty() || Uri.UriSchemeFile.Equals(path.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return DirectoryEntry.Create(type, path);
            }

            if(Uri.UriSchemeFtp.Equals(path.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return FTP.DirectoryEntry.Create(type, path);
            }

            throw new NotSupportedException($"Unsupported URI scheme {path.Scheme}");
        }

        public static T CreateInstance<T>(Path path)
        {
            return (T)CreateInstance(typeof(T), path);
        }
    }
}
