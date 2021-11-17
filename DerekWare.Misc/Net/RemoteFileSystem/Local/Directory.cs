using System.Collections.Generic;
using System.IO;
using System.Linq;
using DerekWare.Collections;
using Path = DerekWare.IO.Path;

namespace DerekWare.Net.RemoteFileSystem.Local
{
    public class Directory : DirectoryEntry<DirectoryInfo>, IDirectory
    {
        public Directory(DirectoryInfo info)
            : base(info)
        {
        }

        public IEnumerable<IDirectoryEntry> Children => Info.GetFileSystemInfos().Select(i => DirectoryEntry.Create(null, new Path(i.FullName))).WhereNotNull();

        public override IDirectory Parent => null != Info.Parent ? new Directory(Info.Parent) : null;
    }
}
