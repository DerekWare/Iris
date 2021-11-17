using System;
using System.IO;
using System.Linq;
using DerekWare.Collections;
using DerekWare.Reflection;
using DerekWare.Strings;
using Path = DerekWare.IO.Path;

namespace DerekWare.Net.RemoteFileSystem.FTP
{
    public struct FileSystemInfo
    {
        public FileAttributes Attributes;
        public DateTime LastWriteTime;
        public long Length;
        public Path Path;

        #region Conversion

        public static FileSystemInfo Parse(Path parent, string contents)
        {
            /*
                drwxrwxrwx   1 owner    group               0 Jul 14 23:45 12 Stones
                -rwxrwxrwx   1 owner    group         6960255 Jul 15  1:35 01 Crash.mp3            
            */

            var parts = contents.Split().WhereNotNull().ToArray();
            var attr = parts[0];
            var size = parts[4];
            var date = parts.Skip(5).Take(3).JoinWords();
            var name = parts.Skip(8).JoinWords();

            return new FileSystemInfo
            {
                Path = parent + name,
                Attributes = attr[0] == 'd' ? FileAttributes.Directory : FileAttributes.Normal,
                Length = size.TryParse<long>(),
                LastWriteTime = date.TryParse<DateTime>()
            };
        }

        #endregion

        public static bool TryParse(Path parent, string contents, out FileSystemInfo entry)
        {
            try
            {
                entry = Parse(parent, contents);
                return true;
            }
            catch
            {
            }

            entry = default;
            return false;
        }
    }
}
