using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using DerekWare.Diagnostics;
using DerekWare.Strings;
using Microsoft.VisualBasic.FileIO;

namespace DerekWare.IO
{
    // TODO recursion
    [Flags]
    public enum FileIOOptions : uint
    {
        None = 0,

        /// <summary>
        ///     Overwrite existing files.
        /// </summary>
        Overwrite = 1u << 0,

        /// <summary>
        ///     Move files rather than copying.
        /// </summary>
        Move = 1u << 1,

        /// <summary>
        ///     Delete files permanently rather than moving to the recycle bin.
        /// </summary>
        DeletePermanently = 1u << 2,

        /// <summary>
        ///     Deletes files in the target directory if they don't exist in the source.
        /// </summary>
        Purge = 1u << 3,

        /// <summary>
        ///     No automatic renaming of files.
        /// </summary>
        PreserveName = 1u << 4,

        /// <summary>
        ///     Test the operation for validity instead of performing it.
        /// </summary>
        Test = 1u << 31
    }

    public delegate bool StreamProgressCallback(long position, long length);

    // TODO maybe these should be extension methods
    public partial class Path
    {
        public const int CopyFileDefaultBufferSize = 16 * 1024 * 1024;

        public void CopyTo(Path target, FileIOOptions options = FileIOOptions.None)
        {
            const FileAttributes attributeMask = FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.System | FileAttributes.Archive;

            Debug.Trace(this, $"CopyTo {target}");

            if(Equals(target))
            {
                throw new IOException("Source and target paths match");
            }

            if(target.Exists && !options.HasFlag(FileIOOptions.Overwrite))
            {
                throw new IOException("The target path already exists");
            }

            var sourceInfo = FileInfo;
            var creationTime = sourceInfo.CreationTimeUtc;
            var writeTime = sourceInfo.LastWriteTimeUtc;
            var attributes = sourceInfo.Attributes & attributeMask;

            if(options.HasFlag(FileIOOptions.Test))
            {
                return;
            }

            if(target.Exists)
            {
                target.Delete(options);
            }
            else
            {
                target.Directory.CreateDirectory();
            }

            CopyFile(this, target);

            var targetInfo = target.FileInfo;
            targetInfo.CreationTimeUtc = creationTime;
            targetInfo.LastWriteTimeUtc = writeTime;
            targetInfo.Attributes |= attributes;

            if(options.HasFlag(FileIOOptions.Move))
            {
                Delete(options);
            }
        }

        public void Delete(FileIOOptions options = FileIOOptions.None)
        {
            Debug.Trace(this, "Delete");

            if(options.HasFlag(FileIOOptions.Test))
            {
                return;
            }

            Attributes &= ~FileAttributes.ReadOnly;

            if(!options.HasFlag(FileIOOptions.DeletePermanently) && FileExists)
            {
                FileSystem.DeleteFile(this, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }
            else if(DirectoryExists)
            {
                // TODO recursion
                System.IO.Directory.Delete(this);
            }
            else
            {
                File.Delete(this);
            }
        }

        public void MoveTo(Path targetPath, FileIOOptions options = FileIOOptions.None)
        {
            CopyTo(targetPath, options | FileIOOptions.Move);
        }

        public static void CopyFile(
            Path src,
            Path dst,
            StreamProgressCallback progressCallback = null,
            FileMode fileMode = FileMode.Create,
            int bufferSize = CopyFileDefaultBufferSize)
        {
            using(var ss = new FileStream(src, FileMode.Open))
            using(var ds = new FileStream(dst, fileMode))
            {
                CopyFile(ss, ds, progressCallback, bufferSize);
            }
        }

        public static bool CopyFile(Stream src, Stream dst, StreamProgressCallback progressCallback = null, int bufferSize = CopyFileDefaultBufferSize)
        {
            if(progressCallback?.Invoke(src.Position, src.Length) == false)
            {
                return false;
            }

            src.Seek(0, SeekOrigin.Begin);
            dst.Seek(0, SeekOrigin.Begin);

            bufferSize = (int)Math.Min(bufferSize, src.Length);
            var buffer = new byte[bufferSize];
            int i;

            while((i = src.Read(buffer, 0, bufferSize)) > 0)
            {
                dst.Write(buffer, 0, i);

                if(progressCallback?.Invoke(src.Position, src.Length) == false)
                {
                    return false;
                }
            }

            dst.SetLength(src.Length);

            return true;
        }

        public static FileStream CreateTempFile(Path parentPath = null, string extension = null)
        {
            var prefix = Assembly.GetEntryAssembly().GetName().Name;

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

    public class StreamProgressEventArgs : CancelEventArgs
    {
        public readonly long Length;
        public readonly long Position;

        public StreamProgressEventArgs(long position, long length)
        {
            Position = position;
            Length = length;
        }
    }
}
