using System;
using System.IO;
using Path = DerekWare.IO.Path;

namespace DerekWare.Net.RemoteFileSystem.Local
{
    public abstract class DirectoryEntry<T> : IDirectoryEntry
        where T : FileSystemInfo
    {
        public abstract IDirectory Parent { get; }
        public readonly T Info;

        protected DirectoryEntry(T info)
        {
            Info = info;
        }

        public virtual FileAttributes Attributes => Info.Attributes;
        public virtual DateTime LastWriteTime => Info.LastWriteTime;
        public virtual string Name => Info.Name;
        public virtual Path Path => new Path(Info.FullName);

        public override string ToString()
        {
            return Info.FullName;
        }
    }

    public static class DirectoryEntry
    {
        public static IDirectoryEntry Create(Type type, Path path)
        {
            return path.Attributes.HasFlag(FileAttributes.Directory) ? (IDirectoryEntry)new Directory(new DirectoryInfo(path)) : new File(new FileInfo(path));
        }

        public static T Create<T>(Path path)
            where T : IDirectoryEntry
        {
            return (T)Create(typeof(T), path);
        }
    }
}
