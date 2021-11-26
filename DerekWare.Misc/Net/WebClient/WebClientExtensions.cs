using System.IO;
using System.Net;
using System.Net.Cache;
using DerekWare.Collections;
using Path = DerekWare.IO.Path;

namespace DerekWare.Net
{
    public static class WebClientExtensions
    {
        public static WebClient CreateWebClient(this Path path, RequestCacheLevel cacheLevel = RequestCacheLevel.Default)
        {
            return new WebClient { BaseAddress = path, CachePolicy = new RequestCachePolicy(cacheLevel) };
        }

        public static WebRequest CreateWebRequest(this Path path, RequestCacheLevel cacheLevel = WebClient.DefaultCacheLevel)
        {
            var request = WebRequest.Create(path.ToString());
            request.CachePolicy = new RequestCachePolicy(cacheLevel);

            if(request is FtpWebRequest ftp)
            {
                ftp.UseBinary = true;
                ftp.UsePassive = true;
                ftp.KeepAlive = true;
            }
            else
            {
                request.UseDefaultCredentials = true;
                request.Proxy.Credentials = request.Credentials;
            }

            return request;
        }

        public static string InvokeWebMethod(this Path path, string method = null, RequestCacheLevel cacheLevel = RequestCacheLevel.Default)
        {
            var request = CreateWebRequest(path, cacheLevel);

            if(!method.IsNullOrEmpty())
            {
                ((FtpWebRequest)request).Method = method;
            }

            using(var response = request.GetResponse())
            using(var stream = response.GetResponseStream())
            using(var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
