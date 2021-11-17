using System.Collections.Generic;
using System.IO;
using System.Linq;
using DerekWare.Collections;

namespace DerekWare.IO
{
    public class FileInfoSet : ObservableHashSet<FileInfo>
    {
        public FileInfoSet()
            : base(FileInfoComparer.Default)
        {
        }

        public FileInfoSet(IEnumerable<FileInfo> collection)
            : base(collection, FileInfoComparer.Default)
        {
        }

        public FileInfoSet(FileInfoCompareFields fields)
            : base(new FileInfoComparer(fields))
        {
        }

        public FileInfoSet(IEnumerable<FileInfo> collection, FileInfoCompareFields fields)
            : base(collection, new FileInfoComparer(fields))
        {
        }

        public FileInfoSet(IEnumerable<Path> collection)
            : this(collection.Select(i => i.FileInfo))
        {
        }

        public FileInfoSet(IEnumerable<Path> collection, FileInfoCompareFields fields)
            : this(collection.Select(i => i.FileInfo), fields)
        {
        }

        public FileInfoSet(
            Path path,
            string searchPattern = null,
            SearchOption searchOption = SearchOption.TopDirectoryOnly,
            FileInfoCompareFields fields = FileInfoCompareFields.All)
            : this(path.GetFiles(searchPattern, searchOption), fields)
        {
        }
    }
}
