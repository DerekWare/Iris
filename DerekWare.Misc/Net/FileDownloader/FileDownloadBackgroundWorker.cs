using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Cache;
using DerekWare.IO;

namespace DerekWare.Net
{
    /// <summary>
    ///     Hosts FileDownloader in a BackgroundWorker.
    /// </summary>
    public class FileDownloadBackgroundWorker : Component
    {
        protected struct Context
        {
            public List<Path> SourcePaths;
            public Path TargetPath;
            public FileDownloadTargetTypes TargetType;
        }

        protected readonly BackgroundWorker BackgroundWorker = new() { WorkerReportsProgress = true };
        protected readonly FileDownloader Downloader = new();

        public event FileDownloadProgressEventHandler DownloadCompleted
        {
            add => Downloader.DownloadCompleted += value;
            remove => Downloader.DownloadCompleted -= value;
        }

        public event FileDownloadProgressEventHandler DownloadProgress
        {
            add => Downloader.DownloadProgress += value;
            remove => Downloader.DownloadProgress -= value;
        }

        public event ProgressChangedEventHandler ProgressChanged;
        public event RunWorkerCompletedEventHandler RunWorkerCompleted;

        public FileDownloadBackgroundWorker()
        {
            BackgroundWorker.DoWork += OnDoWork;
            BackgroundWorker.ProgressChanged += OnProgressChanged;
            BackgroundWorker.RunWorkerCompleted += OnRunWorkerCompleted;
            Downloader.DownloadProgress += OnDownloadProgress;
        }

        public bool CancellationPending => Downloader.Cancelled;
        public bool IsBusy => BackgroundWorker.IsBusy;
        public RequestCacheLevel CacheLevel { get => Downloader.CacheLevel; set => Downloader.CacheLevel = value; }

        public void CancelAsync()
        {
            Downloader.Cancel();
        }

        public void RunWorkerAsync(IEnumerable<Path> sourcePaths, Path targetPath, FileDownloadTargetTypes targetType)
        {
            BackgroundWorker.RunWorkerAsync(new Context { SourcePaths = sourcePaths.ToList(), TargetPath = new Path(targetPath), TargetType = targetType });
        }

        #region Event Handlers

        protected virtual void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var d = (FileDownloadProgressEventArgs)e.UserState;
            ProgressChanged?.Invoke(this, new ProgressChangedEventArgs((d.CurrentIndex * 100) / d.TotalCount, null));
        }

        protected virtual void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(!(e.Error is null) && RunWorkerCompleted is null)
            {
                throw e.Error;
            }

            RunWorkerCompleted?.Invoke(this, new RunWorkerCompletedEventArgs(e.Result, e.Error, e.Cancelled || Downloader.Cancelled));
        }

        void OnDownloadProgress(object sender, FileDownloadProgressEventArgs e)
        {
            BackgroundWorker.ReportProgress(0, e);
        }

        void OnDoWork(object sender, DoWorkEventArgs e)
        {
            var ctx = (Context)e.Argument;
            Downloader.Run(ctx.SourcePaths, ctx.TargetPath, ctx.TargetType);
        }

        #endregion
    }
}
