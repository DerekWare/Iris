using System;
using System.IO;
using System.Net.Cache;
using DerekWare.Collections;
using DerekWare.Diagnostics;
using DerekWare.IO;
using Path = DerekWare.IO.Path;

namespace DerekWare.Net.RemoteFileSystem
{
    public static class RemoteFileCache
    {
        public enum Result
        {
            Exists,
            Create
        }

        public static readonly Path CacheLocation = Path.GetTempPath();

        static readonly object LockContext = new object();
        static readonly ValueMap<Path, Path> ValueMap = new ValueMap<Path, Path>();

        static RemoteFileCache()
        {
            AppDomain.CurrentDomain.ProcessExit += ProcessExit;
        }

        #region Event Handlers

        static void ProcessExit(object sender, EventArgs e)
        {
            try
            {
                CacheLocation.GetFileSystemEntries(null, SearchOption.AllDirectories).ForEach(i => i.Delete(FileIOOptions.DeletePermanently));
            }
            catch(Exception ex)
            {
                Debug.Error(typeof(RemoteFileCache), ex);
            }
        }

        #endregion

        public static Path DownloadFile(Path remotePath, bool reload = false)
        {
            var result = GetLocalFileName(remotePath, out var fileName);

            if((Result.Create == result) || reload)
            {
                using(var client = remotePath.CreateWebClient(RequestCacheLevel.Reload))
                {
                    client.DownloadFile(string.Empty, fileName);
                }
            }

            return fileName;
        }

        public static Result GetLocalFileName(Path remotePath, out Path fileName)
        {
            lock(LockContext)
            {
                if(ValueMap.TryGetRight(remotePath, out fileName) && fileName.FileExists)
                {
                    return Result.Exists;
                }

                var extension = remotePath.Extension;
                fileName = Path.GetTempFileName(CacheLocation, extension);
                ValueMap.Add(remotePath, fileName);
            }

            return Result.Create;
        }
    }
}
