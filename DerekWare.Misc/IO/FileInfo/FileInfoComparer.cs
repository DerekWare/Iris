using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DerekWare.Collections;

namespace DerekWare.IO
{
    [Flags]
    public enum FileInfoCompareFields : uint
    {
        FileName = 1 << 0,
        LastWriteTime = 1 << 1,
        Length = 1 << 2,
        Attributes = 1 << 3,
        All = uint.MaxValue
    }

    public class FileInfoComparer : IComparer<FileInfo>, IComparer<Path>, IEqualityComparer<FileInfo>, IEqualityComparer<Path>
    {
        public static readonly FileInfoComparer Default = new();

        static readonly long FileTimeThreshold = TimeSpan.FromSeconds(2).Ticks;

        public readonly FileInfoCompareFields Fields;

        /// <summary>
        ///     If true, file times within 2 seconds of each other will be considered equal due to FAT file time resolution.
        /// </summary>
        public bool FileTimeCompatibility = true;

        public FileInfoComparer()
            : this(FileInfoCompareFields.All)
        {
        }

        public FileInfoComparer(FileInfoCompareFields fields)
        {
            Fields = fields;
        }

        /// <summary>
        ///     Compares a collection of FileInfo tuples.
        /// </summary>
        /// <returns>The tuples in which Item1 and Item2 are different.</returns>
        public IEnumerable<Tuple<FileInfo, FileInfo>> Diff(IEnumerable<Tuple<FileInfo, FileInfo>> items)
        {
            return items.Where(item => !Equals(item.Item1, item.Item2));
        }

        /// <summary>
        ///     Compares a collection of FileInfo key/value pairs.
        /// </summary>
        /// <returns>The key/value pairs in which Key and Value are different.</returns>
        public IEnumerable<KeyValuePair<FileInfo, FileInfo>> Diff(IEnumerable<KeyValuePair<FileInfo, FileInfo>> items)
        {
            return items.Where(item => !Equals(item.Key, item.Value));
        }

        #region Equality

        public bool Equals(FileInfo x, FileInfo y)
        {
            return 0 == Compare(x, y);
        }

        public bool Equals(Path x, Path y)
        {
            return Equals(x?.FileInfo, y?.FileInfo);
        }

        public int GetHashCode(FileInfo obj)
        {
            var fields = new List<object>();

            if(Fields.HasFlag(FileInfoCompareFields.FileName))
            {
                fields.Add(obj.FullName.ToUpper());
            }

            if(Fields.HasFlag(FileInfoCompareFields.LastWriteTime))
            {
                fields.Add(obj.LastWriteTimeUtc);
            }

            if(Fields.HasFlag(FileInfoCompareFields.Length))
            {
                fields.Add(obj.Length);
            }

            if(Fields.HasFlag(FileInfoCompareFields.Attributes))
            {
                fields.Add(obj.Attributes);
            }

            return SequenceComparer<List<object>>.Default.GetHashCode(fields);
        }

        public int GetHashCode(Path obj)
        {
            return GetHashCode(obj?.FileInfo);
        }

        public int Compare(FileInfo x, FileInfo y)
        {
            if(x is null)
            {
                return y is null ? 0 : 1;
            }

            if(y is null)
            {
                return -1;
            }

            long c = 0;

            if((0 == c) && Fields.HasFlag(FileInfoCompareFields.FileName))
            {
                c = StringComparer.Ordinal.Compare(x.FullName.ToUpper(), y.FullName.ToUpper());
            }

            if((0 == c) && Fields.HasFlag(FileInfoCompareFields.LastWriteTime))
            {
                c = x.LastWriteTimeUtc.Ticks - y.LastWriteTimeUtc.Ticks;

                if(FileTimeCompatibility && (Math.Abs(c) <= FileTimeThreshold))
                {
                    c = 0;
                }
            }

            if((0 == c) && Fields.HasFlag(FileInfoCompareFields.Length))
            {
                c = x.Length - y.Length;
            }

            if((0 == c) && Fields.HasFlag(FileInfoCompareFields.Attributes))
            {
                c = x.Attributes - y.Attributes;
            }

            return (int)c.Clamp(int.MinValue, int.MaxValue);
        }

        public int Compare(Path x, Path y)
        {
            return Compare(x?.FileInfo, y?.FileInfo);
        }

        #endregion
    }
}
