namespace DerekWare.Net.RemoteFileSystem.FTP
{
    public class File : DirectoryEntry, IFile
    {
        public File(FileSystemInfo info)
            : base(info)
        {
        }
    }
}
