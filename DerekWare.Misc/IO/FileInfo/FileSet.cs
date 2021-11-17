using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DerekWare.Collections;

namespace DerekWare.IO
{
    /// <summary>
    ///     Represents a collection of unique files with a common root directory. The files are stored by path relative to the
    ///     root.
    /// </summary>
    public class FileSet : ObservableHashSet<Path>
    {
        public readonly Path Root;

        public FileSet()
        {
        }

        public FileSet(Path root)
        {
            Root = root;
        }

        public FileSet(Path root, IEnumerable<Path> files)
            : this(root)
        {
            AddRange(files.SafeEmpty());
        }

        public FileSet(IReadOnlyCollection<Path> files)
            : this(Path.GetRootDirectory(files), files)
        {
        }

        public FileSet(Path root, string searchPattern, SearchOption searchOption)
            : this(root)
        {
            AddRange(Root.GetFiles(searchPattern, searchOption));
        }

        public FileSet(Path root, IEnumerable<string> searchPatterns, SearchOption searchOption)
            : this(root)
        {
            AddRange(searchPatterns.SelectMany(searchPattern => Root.GetFiles(searchPattern, searchOption)));
        }

        public IEnumerable<Path> AbsolutePaths => this.Select(i => Root.GetAbsolutePath(i));
        public IEnumerable<FileInfo> FileInfo => AbsolutePaths.Select(i => i.FileInfo);
        public IEnumerable<Path> RelativePaths => this;

        public override bool Add(Path item)
        {
            if(item.IsAbsolute)
            {
                if(Root.IsNullOrEmpty())
                {
                    throw new ArgumentException("Can't add an absolute path when the root is null", nameof(item));
                }

                item = Root.GetRelativePath(item);
            }

            return base.Add(item);
        }
    }

    public static partial class PathExtensions
    {
        public static FileSet ToFileSet(this IEnumerable<Path> files, Path root)
        {
            return new FileSet(root, files);
        }

        public static FileSet ToFileSet(this IReadOnlyCollection<Path> files)
        {
            return new FileSet(files);
        }
    }
}
