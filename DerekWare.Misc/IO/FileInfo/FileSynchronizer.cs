using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DerekWare.Collections;
using DerekWare.Threading;

namespace DerekWare.IO
{
    public enum FileSynchronizationAction
    {
        Copying,
        Deleting
    }

    public class FileSynchronizationEventArgs : EventArgs
    {
        public readonly FileSynchronizationAction Action;
        public readonly Path SourcePath;
        public readonly Path TargetPath;
        public bool Cancel;
        public bool Skip;

        public FileSynchronizationEventArgs(Path sourcePath, Path targetPath, FileSynchronizationAction action)
        {
            SourcePath = sourcePath;
            TargetPath = targetPath;
            Action = action;
        }
    }

    public class FileSynchronizer
    {
        public FileInfoCompareFields CompareFields = FileInfoCompareFields.All;
        public FileIOOptions Options = FileIOOptions.None;
        public FileSet Source;
        public FileSet Target;
        public TaskFactory TaskFactory;

        public event EventHandler<FileSynchronizationEventArgs> Synchronizing;

        public FileSynchronizer()
        {
            Source = new FileSet();
            Target = new FileSet();
        }

        public FileSynchronizer(Path source, Path target)
        {
            Source = new FileSet(source);
            Target = new FileSet(target);
        }

        public FileSynchronizer(Path source, Path target, string searchPattern, SearchOption searchOption)
        {
            Source = new FileSet(source, searchPattern, searchOption);
            Target = new FileSet(target, searchPattern, searchOption);
        }

        public FileSynchronizer(Path source, Path target, IReadOnlyCollection<string> searchPatterns, SearchOption searchOption)
        {
            Source = new FileSet(source, searchPatterns, searchOption);
            Target = new FileSet(target, searchPatterns, searchOption);
        }

        /// <summary>
        ///     Compares two sets for files that are missing or different.
        /// </summary>
        /// <returns>
        ///     An enumerable of tuples representing the changed files. If Item1 is null, the file only exists in
        ///     Target. If Item2 is null, the file only exists in Source. If Item1 and Item2 are both non-null, the
        ///     files exist both sets and are different based on CompareFields.
        /// </returns>
        public IEnumerable<Tuple<Path, Path>> Diff
        {
            get
            {
                // Build local sets of the relative file paths
                var source = Source.RelativePaths.ToHashSet();
                var target = Target.RelativePaths.ToHashSet();

                // Get the intersection of matching file names
                var intersect = source.Intersect(target).ToHashSet();

                // Remove all the matches to produce a list of files that aren't in each path. This is the same as using
                // HashSet.Union, except that we need to preserve which directory the file is in.
                source.ExceptWith(intersect);
                target.ExceptWith(intersect);

                // Return all files unique to each folder
                foreach(var i in target)
                {
                    yield return new Tuple<Path, Path>(null, TargetPath.GetAbsolutePath(i));
                }

                foreach(var i in source)
                {
                    yield return new Tuple<Path, Path>(SourcePath.GetAbsolutePath(i), null);
                }

                // Compare the intersecting files, ignoring the file names since they've already been compared
                var c = new FileInfoComparer(CompareFields & ~FileInfoCompareFields.FileName);

                foreach(var i in intersect)
                {
                    var t = new Tuple<Path, Path>(SourcePath.GetAbsolutePath(i), TargetPath.GetAbsolutePath(i));

                    if(!c.Equals(t.Item1, t.Item2))
                    {
                        yield return t;
                    }
                }
            }
        }

        public Path SourcePath
        {
            get => Source.Root;
            set
            {
                if(value.IsNullOrEmpty())
                {
                    throw new ArgumentNullException();
                }

                if(Source?.Root == value)
                {
                    return;
                }

                if(!Source.IsNullOrEmpty())
                {
                    throw new InvalidOperationException("Source path already set");
                }

                Source = new FileSet(value);
            }
        }

        public Path TargetPath
        {
            get => Target.Root;
            set
            {
                if(value.IsNullOrEmpty())
                {
                    throw new ArgumentNullException();
                }

                if(Target?.Root == value)
                {
                    return;
                }

                if(!Target.IsNullOrEmpty())
                {
                    throw new InvalidOperationException("Target path already set");
                }

                Target = new FileSet(value);
            }
        }

        /// <summary>
        ///     Performs a one-directional synchronization of a file set. Files that exist in the source are copied to the target,
        ///     overwriting any that exist. Any files that exist in the target but not the source are deleted.
        /// </summary>
        /// <returns>
        ///     True if the synchronization operation completed; false if it was canceled.
        /// </returns>
        public int Synchronize()
        {
            var count = Diff.TakeWhile(i => Synchronize(i.Item1, i.Item2)).Count();
            TaskFactory?.WaitAll();
            return count;
        }

        protected virtual bool OnCopy(Path src, Path dst, bool currentThread = false)
        {
            if(!currentThread && !(TaskFactory is null))
            {
                TaskFactory.Add(() => OnCopy(src, dst, true));
                return true;
            }

            var e = new FileSynchronizationEventArgs(src, dst, FileSynchronizationAction.Copying);
            Synchronizing?.Invoke(null, e);

            if(e.Cancel)
            {
                return false;
            }

            if(!e.Skip)
            {
                src.CopyTo(dst, Options);
            }

            return true;
        }

        protected virtual bool OnDelete(Path src, Path dst, bool currentThread = false)
        {
            if(!currentThread && !(TaskFactory is null))
            {
                TaskFactory.Add(() => OnDelete(src, dst, true));
                return true;
            }

            var e = new FileSynchronizationEventArgs(src, dst, FileSynchronizationAction.Deleting);
            Synchronizing?.Invoke(null, e);

            if(e.Cancel)
            {
                return false;
            }

            if(!e.Skip)
            {
                dst.Delete(Options);
            }

            return true;
        }

        protected virtual bool Synchronize(Path src, Path dst)
        {
            // TODO bidirectional synchronization
            if(src.IsNullOrEmpty())
            {
                if(Options.HasFlag(FileIOOptions.Purge))
                {
                    if(!OnDelete(src, dst))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if(dst.IsNullOrEmpty() || !Options.HasFlag(FileIOOptions.PreserveName))
                {
                    dst = SourcePath.GetRelativePath(src);
                    dst = TargetPath.GetAbsolutePath(dst);
                }

                if(!dst.Exists || Options.HasFlag(FileIOOptions.Overwrite))
                {
                    if(!OnCopy(src, dst))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
