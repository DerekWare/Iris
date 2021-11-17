using System.Collections.Generic;
using System.Net;
using DerekWare.Strings;

namespace DerekWare.Net.RemoteFileSystem.FTP
{
    public class Directory : DirectoryEntry, IDirectory
    {
        public Directory(FileSystemInfo info)
            : base(info)
        {
        }

        public IEnumerable<IDirectoryEntry> Children
        {
            get
            {
                string response;

                try
                {
                    response = Info.Path.InvokeWebMethod(WebRequestMethods.Ftp.ListDirectoryDetails);
                }
                catch
                {
                    yield break;
                }

                foreach(var content in response.SplitLines(StringSplitOptions.RemoveEmptyEntries))
                {
                    if(FileSystemInfo.TryParse(Info.Path, content, out var info))
                    {
                        yield return Create(info);
                    }
                }
            }
        }
    }
}
