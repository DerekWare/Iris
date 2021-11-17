using System.IO;

namespace DerekWare.Net.RemoteFileSystem.Local
{
    public class File : DirectoryEntry<FileInfo>, IFile
    {
        public File(FileInfo info)
            : base(info)
        {
        }

        public long Length => Info.Length;
        public override IDirectory Parent => new Directory(Info.Directory);
    }
}
