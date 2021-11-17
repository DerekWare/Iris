using System;
using System.Collections.Generic;
using System.Net.Cache;
using DerekWare.Diagnostics;
using DerekWare.IO;

namespace DerekWare.Net
{
    public enum FileDownloadTargetTypes
    {
        /// <summary>
        ///     Download all files to the given folder, preserving file names.
        /// </summary>
        Folder,

        /// <summary>
        ///     Download all files to a single concatenated binary.
        /// </summary>
        ConcatenatedFile
    }

    public delegate void FileDownloadProgressEventHandler(object sender, FileDownloadProgressEventArgs e);

    /// <summary>
    ///     Synchronous file download helper.
    /// </summary>
    public class FileDownloader
    {
        public event FileDownloadProgressEventHandler DownloadCompleted;
        public event FileDownloadProgressEventHandler DownloadProgress;

        public RequestCacheLevel CacheLevel { get; set; } = RequestCacheLevel.Revalidate;
        public bool Cancelled { get; protected set; }

        public void Cancel()
        {
            Cancelled = true;
        }

        public void Run(IReadOnlyCollection<Path> sourcePaths, Path targetPath, FileDownloadTargetTypes targetType)
        {
            Cancelled = false;

            switch(targetType)
            {
                case FileDownloadTargetTypes.Folder:
                    DownloadToFolder(sourcePaths, targetPath);
                    break;

                case FileDownloadTargetTypes.ConcatenatedFile:
                    DownloadToFile(sourcePaths, targetPath);
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        protected void DownloadToFile(IReadOnlyCollection<Path> sourcePaths, Path targetPath)
        {
            var count = sourcePaths.Count;
            var index = 0;

            using(var targetStream = targetPath.OpenWrite())
            using(var sourcePath = sourcePaths.GetEnumerator())
            {
                while(!Cancelled && sourcePath.MoveNext())
                {
                    Debug.Trace(this, $"Downloading {sourcePath.Current} to {targetPath}");

                    DownloadProgress?.Invoke(this,
                                             new FileDownloadProgressEventArgs
                                             {
                                                 CurrentIndex = index, CurrentPath = sourcePath.Current, TargetPath = targetPath, TotalCount = count
                                             });

                    var data = sourcePath.Current.ReadBytes(CacheLevel);
                    targetStream.Write(data, 0, data.Length);
                    ++index;
                }
            }

            DownloadCompleted?.Invoke(this, new FileDownloadProgressEventArgs { CurrentIndex = index, TargetPath = targetPath, TotalCount = count });
        }

        protected void DownloadToFolder(IReadOnlyCollection<Path> sourcePaths, Path targetPath)
        {
            var count = sourcePaths.Count;
            var index = 0;

            using(var sourcePath = sourcePaths.GetEnumerator())
            {
                while(!Cancelled && sourcePath.MoveNext())
                {
                    var t = new Path(targetPath, sourcePath.Current.FileName);
                    Debug.Trace(this, $"Downloading {sourcePath.Current} to {t}");

                    DownloadProgress?.Invoke(this,
                                             new FileDownloadProgressEventArgs
                                             {
                                                 CurrentIndex = index, CurrentPath = sourcePath.Current, TargetPath = t, TotalCount = count
                                             });

                    t.Write(sourcePath.Current.ReadBytes(CacheLevel));
                    ++index;
                }
            }

            DownloadCompleted?.Invoke(this, new FileDownloadProgressEventArgs { CurrentIndex = index, TargetPath = targetPath, TotalCount = count });
        }
    }

    public class FileDownloadProgressEventArgs : EventArgs
    {
        public int CurrentIndex { get; internal set; }
        public Path CurrentPath { get; internal set; }
        public Path TargetPath { get; internal set; }
        public int TotalCount { get; internal set; }
    }
}
