using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DerekWare.IO
{
    static internal class Synchronize
    {
        /// <summary>
        ///     Compares two sets for files that are missing or different.
        /// </summary>
        /// <param name="x">The FileSet to compare.</param>
        /// <param name="y">The FileSet to compare.</param>
        /// <param name="compareFields">
        ///     Fields to compare when determining whether files are different. If compareFields is 0 or
        ///     FileName, CompareDirectories will return all files that exist in both directories.
        /// </param>
        /// <returns>
        ///     An enumerable of tuples representing the changed files. If Item1 is null, the file only exists in
        ///     <paramref name="y" />. If Item2 is null, the file only exists in this. If Item1 and Item2 are both non-null, the
        ///     files exist both sets and are different based on <paramref name="compareFields" />.
        /// </returns>
        public static IEnumerable<Tuple<FileInfo, FileInfo>> Compare(FileSet x, FileSet y, FileInfoCompareFields compareFields = FileInfoCompareFields.All)
        {
            if(!x.Root.DirectoryExists)
            {
                throw new ArgumentException($"{x.Root} is not a directory", nameof(FileSet.Root));
            }

            if(!y.Root.DirectoryExists)
            {
                throw new ArgumentException($"{y.Root} is not a directory", nameof(y.Root));
            }

            // Get the intersection of matching filenames
            var intersect = x.Intersect(y).ToHashSet();

            // Remove all the matches to produce a list of files that aren't in each source path. This is the
            // same as using HashSet.Union, except that we need to preserve which directory the file is in.
            x.ExceptWith(intersect);
            y.ExceptWith(intersect);

            // Return all files unique to each folder
            foreach(var i in x)
            {
                yield return new Tuple<FileInfo, FileInfo>(x.Root.GetAbsolutePath(i).FileInfo, null);
            }

            foreach(var i in y)
            {
                yield return new Tuple<FileInfo, FileInfo>(null, y.Root.GetAbsolutePath(i).FileInfo);
            }

            // Compare the intersecting files, ignoring the filenames since they've already been compared
            var c = new FileInfoComparer(compareFields & ~FileInfoCompareFields.FileName);

            foreach(var i in intersect)
            {
                var t = new Tuple<FileInfo, FileInfo>(x.Root.GetAbsolutePath(i).FileInfo, y.Root.GetAbsolutePath(i).FileInfo);

                if(c.Equals(t.Item1, t.Item2))
                {
                    // The files are the same
                    continue;
                }

                yield return t;
            }
        }

        /// <summary>
        ///     Compares two directories for files that are missing or different.
        /// </summary>
        /// <param name="x">The left directory to compare.</param>
        /// <param name="y">The right directory to compare.</param>
        /// <param name="searchPattern">The search pattern for files to find in each directory.</param>
        /// <param name="searchOption">File search options.</param>
        /// <param name="compareFields">
        ///     Fields to compare when determining whether files are different. If compareFields is 0 or
        ///     FileName, CompareDirectories will return all files that exist in both directories.
        /// </param>
        /// <returns>
        ///     An enumerable of tuples representing the changed files. If Item1 is null, the file only exists in directory
        ///     <paramref name="y" />. If Item2 is null, the file only exists in directory <paramref name="x" />. If Item1 and
        ///     Item2 are both non-null, the files exist both directories and are different based on
        ///     <paramref name="compareFields" />.
        /// </returns>
        public static IEnumerable<Tuple<FileInfo, FileInfo>> Compare(
            Path x,
            Path y,
            string searchPattern = null,
            SearchOption searchOption = SearchOption.TopDirectoryOnly,
            FileInfoCompareFields compareFields = FileInfoCompareFields.All)
        {
            var a = new FileSet(x, searchPattern, searchOption);
            var b = new FileSet(y, searchPattern, searchOption);
            return Compare(a, b, compareFields);
        }

        /// <summary>
        ///     Compares two directories for files that are missing or different.
        /// </summary>
        /// <param name="x">The left directory to compare.</param>
        /// <param name="y">The right directory to compare.</param>
        /// <param name="searchPatterns">The search patterns for files to find in each directory.</param>
        /// <param name="searchOption">File search options.</param>
        /// <param name="compareFields">
        ///     Fields to compare when determining whether files are different. If compareFields is 0 or
        ///     FileName, CompareDirectories will return all files that exist in both directories.
        /// </param>
        /// <returns>
        ///     An enumerable of tuples representing the changed files. If Item1 is null, the file only exists in directory
        ///     <paramref name="y" />. If Item2 is null, the file only exists in directory <paramref name="x" />. If Item1 and
        ///     Item2 are both non-null, the files exist both directories and are different based on
        ///     <paramref name="compareFields" />.
        /// </returns>
        public static IEnumerable<Tuple<FileInfo, FileInfo>> Compare(
            Path x,
            Path y,
            ICollection<string> searchPatterns,
            SearchOption searchOption = SearchOption.TopDirectoryOnly,
            FileInfoCompareFields compareFields = FileInfoCompareFields.All)
        {
            var a = new FileSet(x, searchPatterns, searchOption);
            var b = new FileSet(y, searchPatterns, searchOption);
            return Compare(a, b, compareFields);
        }

        /// <summary>
        ///     Performs a one-directional synchronization of a file set. Files that exist in the source are copied to the target,
        ///     overwriting any that exist. Any files that exist in the target but not the source are deleted.
        /// </summary>
        /// <param name="x">The source file set.</param>
        /// <param name="y">The target file set.</param>
        /// <param name="options">Operation options.</param>
        /// <param name="eventHandler">Event handler called for each file action performed.</param>
        /// <param name="compareFields">
        ///     Fields to compare when determining whether files are different. If compareFields is 0 or
        ///     FileName, CompareDirectories will return all files that exist in both directories.
        /// </param>
        public static void Synchronize(
            FileSet x,
            FileSet y,
            FileSynchronizationOptions options,
            EventHandler<FileSynchronizationEventArgs> eventHandler,
            FileInfoCompareFields compareFields = FileInfoCompareFields.All)
        {
            // TODO bidirectional synchronization
            var set = Compare(x, y, compareFields).ToList();
            var io = options.HasFlag(FileSynchronizationOptions.DeletePermanently) ? FileIOOptions.DeletePermanently : 0;

            // Delete all target files that don't exist in the source or will be overwritten
            void onDelete(Path src, Path dst)
            {
                var e = new FileSynchronizationEventArgs(src, dst, FileSynchronizeAction.Delete);
                eventHandler?.Invoke(null, e);
                dst.Delete(io);
            }

            foreach(var (src, dst) in set)
            {
                if(!(src is null) || options.HasFlag(FileSynchronizationOptions.Purge))
                {
                    onDelete(null, new Path(dst.FullName));
                }
            }

            // Copy source files to target. We always recreate the target filename to handle case changes. Windows won't let you rename a file with only different case.
            void onCopy(Path src)
            {
                var dst = x.Root.GetRelativePath(src);
                dst = y.Root.GetAbsolutePath(dst);
                var e = new FileSynchronizationEventArgs(src, dst, FileSynchronizeAction.Create);
                eventHandler?.Invoke(null, e);
                src.CopyTo(dst, io);
            }

            foreach(var (src, dst) in set)
            {
                onCopy(new Path(src.FullName));
            }
        }
    }
}