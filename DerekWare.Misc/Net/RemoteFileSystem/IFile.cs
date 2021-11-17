namespace DerekWare.Net.RemoteFileSystem
{
    public interface IFile : IDirectoryEntry
    {
        long Length { get; }
    }
}
