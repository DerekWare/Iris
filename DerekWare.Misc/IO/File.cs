using System;
using System.IO;
using DerekWare.Diagnostics;
using DerekWare.Reflection;
using DerekWare.Strings;
using Microsoft.VisualBasic.FileIO;

namespace DerekWare.IO
{
    [Flags]
    public enum FileIOOptions : uint
    {
        None = 0,
        Overwrite = 1 << 0,
        DeletePermanently = 1 << 1,
        Move = 1 << 2
    }

    public partial class Path
    {
        public void CopyTo(Path targetPath, FileIOOptions options = FileIOOptions.None)
        {
            const FileAttributes attributeMask = FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.System | FileAttributes.Archive;

            if(Equals(targetPath))
            {
                throw new IOException("Source and target paths match");
            }

            if(targetPath.Exists)
            {
                if(!options.HasFlag(FileIOOptions.Overwrite))
                {
                    throw new IOException("The target path already exists");
                }

                targetPath.Delete(options);
            }
            else
            {
                targetPath.Directory.CreateDirectory();
            }

            var sourceInfo = FileInfo;
            var creationTime = sourceInfo.CreationTimeUtc;
            var writeTime = sourceInfo.LastWriteTimeUtc;
            var attributes = sourceInfo.Attributes & attributeMask;

            if(options.HasFlag(FileIOOptions.Move))
            {
                sourceInfo.MoveTo(targetPath);
            }
            else
            {
                sourceInfo.CopyTo(targetPath);
            }

            var targetInfo = targetPath.FileInfo;
            targetInfo.CreationTimeUtc = creationTime;
            targetInfo.LastWriteTimeUtc = writeTime;
            targetInfo.Attributes |= attributes;
        }

        public void CopyTo(FileStream targetFile, FileIOOptions options = FileIOOptions.None)
        {
            const FileAttributes attributeMask = FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.System | FileAttributes.Archive;

            var sourceInfo = FileInfo;
            var creationTime = sourceInfo.CreationTimeUtc;
            var writeTime = sourceInfo.LastWriteTimeUtc;
            var attributes = sourceInfo.Attributes & attributeMask;
            var bytes = File.ReadAllBytes(this);

            targetFile.Write(bytes, 0, bytes.Length);
            targetFile.SetLength(bytes.Length);

            var targetInfo = new FileInfo(targetFile.Name) { CreationTimeUtc = creationTime, LastWriteTimeUtc = writeTime };
            targetInfo.Attributes |= attributes;

            if(options.HasFlag(FileIOOptions.Move))
            {
                Delete(options);
            }
        }

        public void Delete(FileIOOptions options = FileIOOptions.None)
        {
            Attributes &= ~FileAttributes.ReadOnly;

            if(!options.HasFlag(FileIOOptions.DeletePermanently) && FileExists)
            {
                FileSystem.DeleteFile(this, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }
            else if(DirectoryExists)
            {
                System.IO.Directory.Delete(this);
            }
            else
            {
                File.Delete(this);
            }
        }

        public bool FilesEqual(Path other)
        {
            return FilesEqual(this, other);
        }

        public void MoveTo(Path targetPath, FileIOOptions options = FileIOOptions.None)
        {
            CopyTo(targetPath, options | FileIOOptions.Move);
        }

        #region Equality

        static bool Equals(DateTime x, DateTime y)
        {
            // FAT filesystems are only accurate to within 2 seconds
            return Math.Abs((x - y).TotalSeconds) <= 2;
        }

        #endregion

        public static FileStream CreateTempFile(Path parentPath = null, string extension = null)
        {
            var prefix = DerekWare.Reflection.Reflection.EntryAssemblyName;

            if(parentPath.IsNullOrEmpty())
            {
                parentPath = GetTempPath();
            }

            if(extension.IsNullOrEmpty())
            {
                extension = ".tmp";
            }
            else if(!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            parentPath.CreateDirectory();

            while(true)
            {
                var filePath = parentPath + $"{prefix}_{Guid.NewGuid():N}{extension}";

                try
                {
                    return new FileStream(filePath, FileMode.CreateNew);
                }
                catch(IOException ex)
                {
                    Debug.Error(filePath, ex);
                }
            }
        }

        public static bool FilesEqual(Path x, Path y)
        {
            return FilesEqual(x?.FileInfo, y?.FileInfo);
        }

        public static bool FilesEqual(FileInfo x, FileInfo y)
        {
            if(x is null || y is null)
            {
                return x is null && y is null;
            }

            if(!Equals(x.Length, y.Length))
            {
                Debug.Trace(null, $"{x}.Length != {y}.Length ({x.Length}, {y.Length}");
                return false;
            }

            if(!Equals(x.CreationTimeUtc, y.CreationTimeUtc))
            {
                Debug.Trace(null, $"{x}.CreationTimeUtc != {y}.CreationTimeUtc ({x.CreationTimeUtc}, {y.CreationTimeUtc}");
                return false;
            }

            if(!Equals(x.LastWriteTimeUtc, y.LastWriteTimeUtc))
            {
                Debug.Trace(null, $"{x}.LastWriteTimeUtc != {y}.LastWriteTimeUtc ({x.LastWriteTimeUtc}, {y.LastWriteTimeUtc}");
                return false;
            }

            return true;
        }

        public static Path GetTempFileName(Path parentPath = null, string extension = null)
        {
            using(var fs = CreateTempFile(parentPath, extension))
            {
                return new Path(fs.Name);
            }
        }

        public static string ToValidLocalFileName(string name, char replace = '\0')
        {
            return replace == '\0' ? name.Remove(InvalidFileNameChars).Trim() : name.Replace(InvalidFileNameChars, replace).Trim();
        }
    }
}
