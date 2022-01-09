using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Cache;
using DerekWare.Diagnostics;
using DerekWare.IO;

namespace DerekWare.Net
{
    public partial class WebClient : System.Net.WebClient
    {
        public const RequestCacheLevel DefaultCacheLevel = RequestCacheLevel.Revalidate;
        public static ICredentials DefaultCredentials = CredentialCache.DefaultCredentials;

        readonly LinkedList<Request> Pending = new();

        public event ErrorEventHandler RequestError;
        public event RequestEventHandler RequestStarted;

        public WebClient()
        {
            Credentials = DefaultCredentials;

            DownloadDataCompleted += OnDownloadDataCompleted;
            DownloadFileCompleted += OnDownloadFileCompleted;
            DownloadProgressChanged += OnDownloadProgressChanged;
        }

        public WebClient(RequestCacheLevel cacheLevel = DefaultCacheLevel)
            : this()
        {
            CacheLevel = cacheLevel;
        }

        public WebClient(RequestCachePolicy cachePolicy)
            : this()
        {
            CachePolicy = cachePolicy;
        }

        public WebClient(Path path, RequestCacheLevel cacheLevel = DefaultCacheLevel)
            : this(cacheLevel)
        {
            BaseAddress = path;
        }

        public WebClient(Path path, RequestCachePolicy cachePolicy)
            : this(cachePolicy)
        {
            BaseAddress = path;
        }

        public new bool IsBusy => (Pending.Count >= 0) || base.IsBusy;
        public new Path BaseAddress { get => new(base.BaseAddress); set => base.BaseAddress = value; }
        public RequestCacheLevel CacheLevel { get => CachePolicy.Level; set => CachePolicy = new RequestCachePolicy(value); }

        public new void Dispose()
        {
            CancelAsync();
            base.Dispose();
            Pending.Clear();
        }

        public void Enqueue(DownloadDataRequest request)
        {
            EnqueueInternal(request);
            DownloadDataAsync(request.Path, request);
        }

        public void Enqueue(DownloadFileRequest request)
        {
            if(request.TargetPath.IsNullOrEmpty())
            {
                request.TargetPath = Path.GetTempFileName();
            }

            EnqueueInternal(request);
            DownloadFileAsync(request.Path, request.TargetPath, request);
        }

        protected void OnDownloadDataCompleted(DownloadDataRequest request, byte[] result)
        {
            Debug.Trace(this, $"Download complete: {request}");
            request.RequestCompleted?.Invoke(this, new DownloadDataEventArgs { Request = request, Result = result });
        }

        protected void OnDownloadFileCompleted(DownloadFileRequest request, Path result)
        {
            Debug.Trace(this, $"Download complete: {request}");
            request.RequestCompleted?.Invoke(this, new DownloadFileEventArgs { Request = request, Result = result });
        }

        protected void OnError(Request request, Exception ex)
        {
            Debug.Trace(this, $"Error: {request} {ex}");
            request.RequestError?.Invoke(this, new ErrorEventArgs { Request = request, Exception = ex });
            RequestError?.Invoke(this, new ErrorEventArgs { Request = request, Exception = ex });
        }

        protected void OnStarted(Request request)
        {
            Debug.Trace(this, $"Started: {request}");
            request.RequestStarted?.Invoke(this, new RequestEventArgs { Request = request });
            RequestStarted?.Invoke(this, new RequestEventArgs { Request = request });
        }

        void EnqueueInternal(Request request)
        {
            Pending.AddLast(request);
            OnStarted(request);
        }

        #region Event Handlers

        void OnDownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            var request = e.UserState as DownloadDataRequest;

            if((null == request) || !Pending.Remove(request))
            {
                return;
            }

            if(null != e.Error)
            {
                OnError(request, e.Error);
            }
            else if(!e.Cancelled)
            {
                OnDownloadDataCompleted(request, e.Result);
            }
        }

        void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if(!(e.UserState is DownloadFileRequest request) || !Pending.Remove(request))
            {
                return;
            }

            if(null != e.Error)
            {
                OnError(request, e.Error);
            }
            else if(!e.Cancelled)
            {
                OnDownloadFileCompleted(request, request.TargetPath);
            }
        }

        void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Debug.Trace(this, $"{e.UserState} BytesReceived={e.BytesReceived}, ProgressPercentage={e.ProgressPercentage}");
        }

        #endregion
    }
}
