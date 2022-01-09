using System;
using System.IO;
using Path = DerekWare.IO.Path;

namespace DerekWare.Net.RemoteFileSystem.FTP
{
    public class DirectoryEntry : IDirectoryEntry
    {
        public readonly FileSystemInfo Info;

        public DirectoryEntry(FileSystemInfo info)
        {
            if(!info.Path.Scheme.Equals(Uri.UriSchemeFtp, StringComparison.OrdinalIgnoreCase))
            {
                throw new FormatException("Scheme mismatch");
            }

            Info = info;
        }

        public virtual FileAttributes Attributes => Info.Attributes;
        public DateTime LastWriteTime => Info.LastWriteTime;
        public long Length => Info.Length;
        public virtual string Name => Info.Path.FileNameWithoutExtension;

        public virtual IDirectory Parent
        {
            get
            {
                var parent = Info.Path.Parent;
                return null != parent ? Create<IDirectory>(parent) : null;
            }
        }

        public virtual Path Path => Info.Path;

        public override string ToString()
        {
            return Info.Path;
        }

        public static IDirectoryEntry Create(FileSystemInfo info)
        {
            return info.Attributes.HasFlag(FileAttributes.Directory) ? new Directory(info) : new File(info);
        }

        public static T Create<T>(FileSystemInfo info)
            where T : IDirectoryEntry
        {
            return (T)Create(info);
        }

        public static IDirectoryEntry Create(Type type, Path path)
        {
            if(typeof(IFile).IsAssignableFrom(type))
            {
                return new File(new FileSystemInfo { Path = path });
            }

            if(typeof(IDirectory).IsAssignableFrom(type))
            {
                return new Directory(new FileSystemInfo { Path = path });
            }

            if(typeof(IDirectoryEntry).IsAssignableFrom(type))
            {
                return new DirectoryEntry(new FileSystemInfo { Path = path });
            }

            throw new TypeAccessException("Invalid object or interface type");
        }

        public static T Create<T>(Path path)
            where T : IDirectoryEntry
        {
            return (T)Create(typeof(T), path);
        }
    }
}
