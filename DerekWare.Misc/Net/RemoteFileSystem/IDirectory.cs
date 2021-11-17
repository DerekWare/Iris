using System;
using System.Collections.Generic;
using System.Linq;
using DerekWare.Diagnostics;

namespace DerekWare.Net.RemoteFileSystem
{
    public interface IDirectory : IDirectoryEntry
    {
        IEnumerable<IDirectoryEntry> Children { get; }
    }

    public static class Directory
    {
        public static IEnumerable<IDirectoryEntry> GetChildren(this IDirectory @this, bool recursive = false)
        {
            var result = new List<IDirectoryEntry>();

            foreach(var i in @this.Children)
            {
                result.Add(i);

                if(!recursive || !(i is IDirectory directory))
                {
                    continue;
                }

                try
                {
                    result.AddRange(directory.GetChildren(recursive));
                }
                catch(Exception ex)
                {
                    Debug.Error(@this, ex);
                }
            }

            return result;
        }

        public static List<IDirectory> GetDirectories(this IDirectory @this, bool recursive = false)
        {
            return GetChildren(@this, recursive).OfType<IDirectory>().ToList();
        }

        public static List<IFile> GetFiles(this IDirectory @this, bool recursive = false)
        {
            return GetChildren(@this, recursive).OfType<IFile>().ToList();
        }
    }
}
