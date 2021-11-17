using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DerekWare.Collections;
using DerekWare.Diagnostics;
using DerekWare.Strings;

namespace DerekWare.IO
{
    // TODO maybe these should be extension methods
    public partial class Path
    {
        public static Path ApplicationDataPath => GetSpecialFolderPath(Environment.SpecialFolder.ApplicationData) + Assembly.GetEntryAssembly().GetName().Name;
        public static Path CurrentDirectory => new Path(Environment.CurrentDirectory);
        public static Path SystemDirectory => new Path(Environment.SystemDirectory);

        public bool IsDirectoryEmpty => DirectoryExists && !GetFileSystemEntries().Any();

        public void CreateDirectory()
        {
            System.IO.Directory.CreateDirectory(this);
        }

        public IEnumerable<Path> GetDirectories(string searchPattern = null, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return GetFileSystemEntries(searchPattern, searchOption).Where(v => v.DirectoryExists);
        }

        public IEnumerable<Path> GetFiles(string searchPattern = null, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return GetFileSystemEntries(searchPattern, searchOption).Where(v => v.FileExists);
        }

        public IEnumerable<Path> GetFileSystemEntries(string searchPattern = null, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var file = new Path(searchPattern);

            if(file.Exists)
            {
                try
                {
                    if(GetRootDirectory(this, file) == this)
                    {
                        return file.AsEnumerable();
                    }
                }
                catch(Exception ex)
                {
                    Debug.Trace(this, ex);
                }

                throw new IOException("The search pattern is a file, but that file does not exist under the parent path");
            }

            try
            {
                return new DirectoryInfo(this).GetFileSystemInfos(searchPattern.IfNullOrEmpty("*"), searchOption).Select(ToPath);
            }
            catch(Exception ex)
            {
                Debug.Trace(this, ex);
                return Array.Empty<Path>();
            }
        }

        public int RemoveEmptyDirectories()
        {
            // TODO disable recursion
            var count = GetDirectories().Sum(i => i.RemoveEmptyDirectories());

            if(IsDirectoryEmpty)
            {
                Delete();
                ++count;
            }

            return count;
        }

        public bool TryCreateDirectory()
        {
            try
            {
                CreateDirectory();
                return true;
            }
            catch(Exception ex)
            {
                Debug.Trace(this, ex);
                return false;
            }
        }

        public static Path GetApplicationDataPath(params string[] parts)
        {
            return new Path(parts.Prepend(ApplicationDataPath));
        }

        public static Path GetSpecialFolderPath(Environment.SpecialFolder folder)
        {
            return new Path(Environment.GetFolderPath(folder));
        }

        public static Path GetTempPath(Path parentPath = null)
        {
            parentPath = parentPath ?? new Path(GetTempPath());

            while(true)
            {
                var child = parentPath + Guid.NewGuid().ToString("N");
                var info = new DirectoryInfo(child);

                if(info.Exists)
                {
                    continue;
                }

                info.Create();
                return child;
            }
        }

        /// <summary>
        ///     Splits a search pattern such as "c:\foo\*.bar" to the directory and file search pattern.
        /// </summary>
        /// <param name="searchPattern">On input, the original search pattern. On output, the file search pattern.</param>
        /// <returns>The directory to search.</returns>
        public static Path SplitSearchPattern(ref string searchPattern)
        {
            var path = new Path(searchPattern);

            if(path.DirectoryExists)
            {
                searchPattern = null;
            }
            else
            {
                searchPattern = path.FileName;
                path = path.Parent;
            }

            if(path.IsNullOrEmpty())
            {
                path = CurrentDirectory;
            }

            if(searchPattern.IsNullOrEmpty())
            {
                searchPattern = "*";
            }

            return path;
        }

        public static string ToValidLocalPathName(string name, char replace = '\0')
        {
            return replace == '\0' ? name.Remove(InvalidPathChars).Trim() : name.Replace(InvalidPathChars, replace).Trim();
        }
    }
}
