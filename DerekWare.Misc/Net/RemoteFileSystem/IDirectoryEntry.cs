using System;
using System.IO;
using Path = DerekWare.IO.Path;

namespace DerekWare.Net.RemoteFileSystem
{
    public interface IDirectoryEntry
    {
        FileAttributes Attributes { get; }
        DateTime LastWriteTime { get; }
        string Name { get; }
        IDirectory Parent { get; }
        Path Path { get; }
    }
}
