using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Cache;
using System.Text;
using DerekWare.Strings;
using Path = DerekWare.IO.Path;
using StringSplitOptions = DerekWare.Strings.StringSplitOptions;

namespace DerekWare.Net
{
    public static class PathExtensions
    {
        public static Stream OpenRead(this Path path, RequestCacheLevel cacheLevel = WebClient.DefaultCacheLevel)
        {
            return path.IsLocal ? File.OpenRead(path.Url) : new WebClient(cacheLevel).OpenRead(path.Url);
        }

        public static Stream OpenWrite(this Path path, RequestCacheLevel cacheLevel = WebClient.DefaultCacheLevel)
        {
            return path.IsLocal ? File.OpenWrite(path.Url) : new WebClient(cacheLevel).OpenWrite(path.Url);
        }

        public static byte[] ReadBytes(this Path path, RequestCacheLevel cacheLevel = WebClient.DefaultCacheLevel)
        {
            return path.IsLocal ? File.ReadAllBytes(path.Url) : new WebClient(cacheLevel).DownloadData(path.Url);
        }

        public static string[] ReadLines(this Path path, StringSplitOptions splitOptions = default, RequestCacheLevel cacheLevel = WebClient.DefaultCacheLevel)
        {
            return path.ReadLines(Encoding.Default, splitOptions, cacheLevel);
        }

        public static string[] ReadLines(
            this Path path,
            Encoding encoding,
            StringSplitOptions splitOptions = default,
            RequestCacheLevel cacheLevel = WebClient.DefaultCacheLevel)
        {
            return path.ReadText(encoding, cacheLevel).SplitLines(splitOptions).ToArray();
        }

        public static string ReadText(this Path path, RequestCacheLevel cacheLevel = WebClient.DefaultCacheLevel)
        {
            return path.ReadText(Encoding.Default, cacheLevel);
        }

        public static string ReadText(this Path path, Encoding encoding, RequestCacheLevel cacheLevel = WebClient.DefaultCacheLevel)
        {
            return encoding.GetString(path.ReadBytes(cacheLevel));
        }

        public static void Write(this Path path, byte[] bytes, RequestCacheLevel cacheLevel = WebClient.DefaultCacheLevel)
        {
            if(path.IsLocal)
            {
                File.WriteAllBytes(path.Url, bytes);
            }
            else
            {
                new WebClient(cacheLevel).UploadData(path.Url, bytes);
            }
        }

        public static void Write(this Path path, IEnumerable<string> content, RequestCacheLevel cacheLevel = WebClient.DefaultCacheLevel)
        {
            if(path.IsLocal)
            {
                File.WriteAllLines(path.Url, content);
            }
            else
            {
                new WebClient(cacheLevel).UploadString(path.Url, content.Join(Environment.NewLine));
            }
        }

        public static void Write(this Path path, string content, RequestCacheLevel cacheLevel = WebClient.DefaultCacheLevel)
        {
            if(path.IsLocal)
            {
                File.WriteAllText(path.Url, content);
            }
            else
            {
                new WebClient(cacheLevel).UploadString(path.Url, content);
            }
        }
    }
}
