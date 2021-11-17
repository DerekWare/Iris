using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using DerekWare.Collections;
using DerekWare.Diagnostics;
using DerekWare.Strings;
using StringSplitOptions = DerekWare.Strings.StringSplitOptions;

namespace DerekWare.IO
{
    public enum PathFormat
    {
        Unknown,
        Local,
        Unc,
        Uri
    }

    /// <summary>
    ///     The path class wraps a string and enables a combination of System.IO.Path methods
    ///     as well as Uri methods. The path class works with paths in standard Windows format,
    ///     UNC and Uri.
    /// </summary>
    public partial class Path : IEquatable<Path>, IEquatable<object>, IComparable<Path>, IComparable<object>, IComparable
    {
        public static readonly Path Empty = new Path();
        public static readonly char[] InvalidFileNameChars = System.IO.Path.GetInvalidFileNameChars();
        public static readonly char[] InvalidPathChars = System.IO.Path.GetInvalidPathChars();
        public static readonly char[] PathDelimiters = { System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar };

        public Path()
        {
        }

        /// <summary>
        ///     Build a new path from one or more existing paths.
        /// </summary>
        /// <param name="paths"></param>
        public Path(IEnumerable<Path> paths)
        {
            var _paths = paths.SafeEmpty().ToQueue();

            if(_paths.Count <= 0)
            {
                return;
            }

            var path = _paths.Pop();

            Scheme = path.Scheme;
            Host = path.Host;
            Port = path.Port;
            PathDelimiter = path.PathDelimiter;
            Segments = new SegmentList(path.Segments, PathDelimiter);

            while(_paths.Count > 0)
            {
                path = _paths.Pop();

                if(!path.Scheme.IsNullOrEmpty() && !path.Scheme.Equals(Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    throw new IOException("Can't combine two paths with different schemes");
                }

                if(!path.Host.IsNullOrEmpty() && !path.Host.Equals(Host, StringComparison.OrdinalIgnoreCase))
                {
                    throw new IOException("Can't combine two paths with different hosts");
                }

                if((path.Port > 0) && (path.Port != Port))
                {
                    throw new IOException("Can't combine two paths with different ports");
                }

                if(path.IsAbsolute)
                {
                    path = GetRelativePath(path);
                }

                Segments = new SegmentList(Segments.Append(path.Segments), PathDelimiter);
            }
        }

        /// <summary>
        ///     Parse a string to build a Path.
        /// </summary>
        /// <param name="path"></param>
        public Path(string path)
        {
            path = path ?? string.Empty;
            path = path.Trim('\"');
            path = Uri.UnescapeDataString(path);

            // Determine the format of the path
            var format = GetPathFormat(path);

            // If the format is URI, we may want to break it down even further
            if((format == PathFormat.Uri) && path.StartsWith(Uri.UriSchemeFile + Uri.SchemeDelimiter))
            {
                path = path.TrimStart(Uri.UriSchemeFile + Uri.SchemeDelimiter);
                format = GetPathFormat(path);
            }

            // Split the path into segments based on the standard path delimiters
            var segments = SplitPath(path, StringSplitOptions.RemoveEmptyEntries).ToList();

            // Find the host device
            switch(format)
            {
                case PathFormat.Unc:
                    Host = segments[0];
                    break;

                case PathFormat.Uri:
                    Scheme = segments[0].TrimEnd(':');
                    segments.RemoveAt(0);
                    PathDelimiter = '/';
                    Host = segments[0];
                    break;
            }

            // Try to pull the port out of the host
            if(!Host.IsNullOrEmpty())
            {
                var parts = Host.Split(':');

                if((parts.Length == 2) && (parts[0].Length > 0) && int.TryParse(parts[1], out var port))
                {
                    Host = parts[0];
                    Port = port;
                }
            }

            // Format the root segment
            switch(format)
            {
                case PathFormat.Unc:
                    segments[0] = @"\\";
                    break;

                case PathFormat.Uri:
                    segments[0] = Scheme + Uri.SchemeDelimiter;
                    break;

                default:

                    // Special case a path that starts with a root directory marker
                    if(!path.IsNullOrEmpty() && PathDelimiters.Contains(path[0]))
                    {
                        segments[0] = PathDelimiter + segments[0];
                    }

                    break;
            }

            if(!Host.IsNullOrEmpty())
            {
                segments[0] += Host;

                if(Port > 0)
                {
                    segments[0] += $":{Port}";
                }
            }

            // Build the segment list and URL   
            Segments = new SegmentList(segments, PathDelimiter);
        }

        public Path(IEnumerable<string> paths)
            : this(paths.SafeEmpty().Select(path => new Path(path)))
        {
        }

        public Path(IEnumerable<Uri> paths)
            : this(paths.SafeEmpty().Select(item => new Path(item?.AbsoluteUri)))
        {
        }

        public Path(IEnumerable<FileSystemInfo> paths)
            : this(paths.SafeEmpty().Select(item => new Path(item?.FullName)))
        {
        }

        public Path(params Path[] paths)
            : this((IEnumerable<Path>)paths)
        {
        }

        public Path(params string[] paths)
            : this((IEnumerable<string>)paths)
        {
        }

        public Path(params Uri[] paths)
            : this((IEnumerable<Uri>)paths)
        {
        }

        public Path(params FileSystemInfo[] paths)
            : this((IEnumerable<FileSystemInfo>)paths)
        {
        }

        /// <summary>
        ///     Special version of the constructor used just to rebuild a path with a modified segment list. It's assumed that the
        ///     segments have already been parsed and validated.
        /// </summary>
        protected Path(Path other, IEnumerable<string> segments)
        {
            Scheme = other.Scheme;
            Host = other.Host;
            Port = other.Port;
            PathDelimiter = other.PathDelimiter;
            Segments = new SegmentList(segments, PathDelimiter);
        }

        /// <summary>
        ///     Converts a local file path to an absolute local file path. Has no effect on URI paths.
        /// </summary>
        public Path AbsolutePath => IsEmpty ? new Path(Environment.CurrentDirectory) : IsLocal ? new Path(System.IO.Path.GetFullPath(this)) : new Path(this);

        /// <summary>
        ///     The name of the path's directory component. If the path is a directory, returns the current object.
        /// </summary>
        public Path Directory => DirectoryExists ? new Path(this) : Parent;

        /// <summary>
        ///     True if the path exists and is a local directory.
        /// </summary>
        public bool DirectoryExists
        {
            get
            {
                switch(GetPathFormat(this))
                {
                    case PathFormat.Unknown:
                    case PathFormat.Local:
                        if(!Url.ContainsAny(InvalidPathChars) && !Url.ContainsAny(new[] { "*", "?" }))
                        {
                            try
                            {
                                return System.IO.Directory.Exists(this);
                            }
                            catch(IOException ex)
                            {
                                Debug.Trace(this, ex);
                            }
                        }

                        break;
                }

                return false;
            }
        }

        /// <summary>
        ///     A DirectoryInfo representing the directory, if it exists.
        /// </summary>
        public DirectoryInfo DirectoryInfo => DirectoryExists ? new DirectoryInfo(this) : null;

        /// <summary>
        ///     True if the path exists locally.
        /// </summary>
        public bool Exists
        {
            get
            {
                switch(GetPathFormat(this))
                {
                    case PathFormat.Unknown:
                    case PathFormat.Local:
                        if(!Url.ContainsAny(InvalidPathChars) && !Url.ContainsAny(new[] { "*", "?" }))
                        {
                            try
                            {
                                return File.Exists(this) || System.IO.Directory.Exists(this);
                            }
                            catch(IOException ex)
                            {
                                Debug.Trace(this, ex);
                            }
                        }

                        break;
                }

                return false;
            }
        }

        /// <summary>
        ///     Returns just the extension component of the file name.
        /// </summary>
        public string Extension
        {
            get
            {
                var f = FileName;
                var i = f.LastIndexOf('.');
                return i > 0 ? f.Remove(0, i) : string.Empty;
            }
        }

        /// <summary>
        ///     True if the path exists and is a local file.
        /// </summary>
        public bool FileExists
        {
            get
            {
                switch(GetPathFormat(this))
                {
                    case PathFormat.Unknown:
                    case PathFormat.Local:
                        if(!Url.ContainsAny(InvalidPathChars) && !Url.ContainsAny(new[] { "*", "?" }))
                        {
                            try
                            {
                                return File.Exists(this);
                            }
                            catch(IOException ex)
                            {
                                Debug.Trace(this, ex);
                            }
                        }

                        break;
                }

                return false;
            }
        }

        /// <summary>
        ///     A FileInfo representing the file, if it exists.
        /// </summary>
        public FileInfo FileInfo => FileExists ? new FileInfo(this) : null;

        /// <summary>
        ///     Returns just the file name component of the path.
        /// </summary>
        public string FileName => Segments.Count > 0 ? Segments[Segments.Count - 1] : string.Empty;

        /// <summary>
        ///     Returns just the file name component of the path without the file extension.
        /// </summary>
        public string FileNameWithoutExtension
        {
            get
            {
                var f = FileName;
                var i = f.LastIndexOf('.');
                return i > 0 ? f.Remove(i) : f;
            }
        }

        /// <summary>
        ///     A DirectoryInfo or FileInfo representing the path, if it exists.
        /// </summary>
        public FileSystemInfo FileSystemInfo => (FileSystemInfo)DirectoryInfo ?? FileInfo;

        /// <summary>
        ///     The remote host device.
        /// </summary>
        public string Host { get; } = "";

        /// <summary>
        ///     True if the path is fully-resolved; false if relative.
        /// </summary>
        public bool IsAbsolute => !Host.IsNullOrEmpty() || IsLocalFormat(Root);

        /// <summary>
        ///     True if the path is empty and invalid.
        /// </summary>
        public bool IsEmpty => Url.IsNullOrEmpty();

        /// <summary>
        ///     True if this is a path to a local file or directory or a relative path.
        /// </summary>
        public bool IsLocal => Scheme.IsNullOrEmpty();

        /// <summary>
        ///     True if this is a URI reference to a remote device.
        /// </summary>
        public bool IsUri => !Scheme.IsNullOrEmpty();

        /// <summary>
        ///     Returns true if the path is both local and contains only valid characters.
        /// </summary>
        public bool IsValidLocalPath
        {
            get
            {
                if(!IsLocal)
                {
                    return false;
                }

                var index = 0;

                if(IsAbsolute)
                {
                    if(Segments[index++].IndexOfAny(InvalidPathChars) >= 0)
                    {
                        return false;
                    }
                }

                while(index < Segments.Count)
                {
                    if(Segments[index++].IndexOfAny(InvalidFileNameChars) >= 0)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        ///     The parent directory or null if this is the root.
        /// </summary>
        public Path Parent => Segments.Count > 1 ? new Path(this, Segments.Take(Segments.Count - 1)) : null;

        /// <summary>
        ///     The character used to separate path components based on Style.
        /// </summary>
        public char PathDelimiter { get; } = System.IO.Path.DirectorySeparatorChar;

        /// <summary>
        ///     The host port or 0.
        /// </summary>
        public int Port { get; }

        /// <summary>
        ///     The root directory, which is a combination of Scheme and Host.
        /// </summary>
        public string Root => Segments.Count > 0 ? Segments[0] : null;

        /// <summary>
        ///     The URI scheme.
        /// </summary>
        public string Scheme { get; } = "";

        /// <summary>
        ///     The parts of the local path. Note that this differs from the Uri class in that it includes the root.
        /// </summary>
        public SegmentList Segments { get; } = new SegmentList();

        /// <summary>
        ///     Ensures a local path is valid.
        /// </summary>
        public Path ToValidLocalPath
        {
            get
            {
                if(!IsLocal)
                {
                    throw new IOException("May only be called on local paths");
                }

                var segments = Segments.ToList();
                var index = 0;

                if(IsAbsolute)
                {
                    segments[index] = ToValidLocalPathName(segments[index++]);
                }

                while(index < segments.Count)
                {
                    segments[index] = ToValidLocalFileName(segments[index++]);
                }

                return new Path(this, segments);
            }
        }

        /// <summary>
        ///     The path's complete URL.
        /// </summary>
        public string Url => Segments.Url;

        /// <summary>
        ///     Attributes of the file system entry.
        /// </summary>
        public FileAttributes Attributes
        {
            get
            {
                try
                {
                    return File.GetAttributes(this);
                }
                catch(IOException ex)
                {
                    Debug.Trace(this, ex);
                }

                return 0;
            }
            set => File.SetAttributes(this, value);
        }

        public Path ChangeExtension(string extension)
        {
            if(!extension.IsNullOrEmpty() && !extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            return ChangeFileName(FileNameWithoutExtension + extension);
        }

        public Path ChangeFileName(string fileName)
        {
            return new Path(this, Segments.Take(Segments.Count - 1).Append(fileName));
        }

        public Path ExpandEnvironmentVariables()
        {
            return ToPath(Environment.ExpandEnvironmentVariables(this));
        }

        /// <summary>
        ///     Finds a file in the system path.
        /// </summary>
        public IEnumerable<Path> FindInPath(string env)
        {
            if(FileExists)
            {
                yield return this;
                yield break;
            }

            var f = ToPath(FileName);

            foreach(var p in Environment.ExpandEnvironmentVariables(env).Split(';'))
            {
                var a = ToPath(p).GetAbsolutePath(f);

                if(a.FileExists)
                {
                    yield return a;
                }
            }
        }

        /// <summary>
        ///     Finds a file in the system path.
        /// </summary>
        public IEnumerable<Path> FindInPath()
        {
            return FindInPath(Environment.ExpandEnvironmentVariables("PATH"));
        }

        public Path GetAbsolutePath(Path other)
        {
            return GetAbsolutePath(this, other);
        }

        public int GetDivergence(Path other)
        {
            return GetDivergence(this, other);
        }

        public IEnumerable<Path> GetParents()
        {
            var current = Parent;

            while(!current.IsNullOrEmpty())
            {
                yield return current;
                current = current.Parent;
            }
        }

        public Path GetRelativePath(Path child)
        {
            // Find the common path between this and the parent. The parent path must be a folder.
            if(FileExists)
            {
                throw new IOException("Parent path must be a folder");
            }

            var d = GetDivergence(child);

            if(d <= 0)
            {
                throw new IOException("The paths do not share a common root");
            }

            // Remove the common segments
            var parentSegments = Segments.Skip(d).ToLinkedList();
            var childSegments = child.Segments.Skip(d).ToLinkedList();

            // Insert path tokens
            foreach(var _ in parentSegments)
            {
                childSegments.AddFirst("..");
            }

            // Rebuild the path
            return new Path(childSegments);
        }

        public Path RemoveAt(int index)
        {
            var segments = Segments.ToList();
            segments.RemoveAt(index);
            return new Path(this, segments);
        }

        public Path RemoveFirst()
        {
            return RemoveAt(0);
        }

        public Path RemoveLast()
        {
            return RemoveAt(Segments.Count - 1);
        }

        public FileSystemInfo ToFileSystemInfo()
        {
            return FileSystemInfo;
        }

        public Path ToLocalPath(char replace = '\0')
        {
            var segments = Segments.ToArray();
            var index = 0;

            if(IsAbsolute)
            {
                segments[index++] = segments[index].Replace(InvalidPathChars, replace);
            }

            while(index < segments.Length)
            {
                segments[index++] = segments[index].Replace(InvalidFileNameChars, replace);
            }

            return new Path(this, segments);
        }

        public override string ToString()
        {
            return Url;
        }

        public Uri ToUri()
        {
            if(Uri.TryCreate(this, UriKind.RelativeOrAbsolute, out var result))
            {
                return result;
            }

            Debug.Error(this, "Unable to create URI");
            throw new WebException($"Unable to create URI from {this}");
        }

        #region Equality

        public new static bool Equals(object x, object y)
        {
            var a = ToPath(x);
            var b = ToPath(y);

            if(a.IsNullOrEmpty() || b.IsNullOrEmpty())
            {
                return a.IsNullOrEmpty() == b.IsNullOrEmpty();
            }

            return StringComparer.OrdinalIgnoreCase.Equals(a.Url, b.Url);
        }

        public bool Equals(Path other)
        {
            return Equals(this, other);
        }

        public override bool Equals(object other)
        {
            return Equals(this, other);
        }

        public override int GetHashCode()
        {
            return Url.ToLower().GetHashCode();
        }

        public static int Compare(object x, object y)
        {
            var a = ToPath(x);
            var b = ToPath(y);

            if(ReferenceEquals(null, x) || ReferenceEquals(null, y))
            {
                return ReferenceEquals(null, x) ? 1 : -1;
            }

            return StringComparer.OrdinalIgnoreCase.Compare(a.Url, b.Url);
        }

        #endregion

        #region IComparable<object>

        public int CompareTo(object other)
        {
            return Compare(this, other);
        }

        #endregion

        #region IComparable<Path>

        public int CompareTo(Path other)
        {
            return Compare(this, other);
        }

        #endregion

        public static Path GetAbsolutePath(Path a, Path b)
        {
            return !a.IsAbsolute && b.IsAbsolute ? b + a : a + b;
        }

        public static int GetDivergence(Path left, Path right)
        {
            return SequenceComparer<string>.GetDivergence(left.Segments, right.Segments, StringComparer.OrdinalIgnoreCase);
        }

        public static PathFormat GetPathFormat(string path)
        {
            return IsLocalFormat(path) ? PathFormat.Local : IsUncFormat(path) ? PathFormat.Unc : IsUriFormat(path) ? PathFormat.Uri : PathFormat.Unknown;
        }

        public static Path GetRootDirectory(params Path[] paths)
        {
            return GetRootDirectory((IEnumerable<Path>)paths);
        }

        public static Path GetRootDirectory(IEnumerable<Path> paths)
        {
            Path result = null;

            foreach(var p in paths)
            {
                var path = p.IsLocal ? p.AbsolutePath : p;

                if(result.IsNullOrEmpty())
                {
                    result = path;
                    continue;
                }

                if(!path.Scheme.Equals(result.Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    throw new IOException("The paths do not share a common scheme");
                }

                if(!path.Host.Equals(result.Host, StringComparison.OrdinalIgnoreCase))
                {
                    throw new IOException("The paths do not share a common host");
                }

                if(!path.Port.Equals(result.Port))
                {
                    throw new IOException("The paths do not share a common port");
                }

                var c = Math.Min(result.Segments.Count, path.Segments.Count);
                var i = 0;

                while((i < c) && result.Segments[i].Equals(path.Segments[i], StringComparison.OrdinalIgnoreCase))
                {
                    ++i;
                }

                if(i <= 0)
                {
                    throw new IOException("The paths do not share a common root");
                }

                result = new Path(path.Segments.Take(i));
            }

            return result;
        }

        public static bool IsLocalFormat(string path)
        {
            // d:\foo
            if(path.IsNullOrEmpty() || (path.Length < 2))
            {
                return false;
            }

            if(!char.IsLetter(path[0]) || (':' != path[1]))
            {
                return false;
            }

            if((path.Length > 2) && ('\\' != path[2]) && ('/' != path[2]))
            {
                return false;
            }

            return true;
        }

        public static bool IsUncFormat(string path)
        {
            // \\host
            return !path.IsNullOrEmpty() && (path.StartsWith(@"\\") || path.StartsWith(@"//"));
        }

        public static bool IsUriFormat(string path)
        {
            // scheme://host:port
            return !path.IsNullOrEmpty() && ((path.IndexOf(@"://") > 1) || (path.IndexOf(@":\\") > 1));
        }

        public static IEnumerable<string> ResolvePathSegments(IEnumerable<string> segments)
        {
            var r = segments.SafeEmpty().ToLinkedList();
            var c = r.First;

            while(null != c)
            {
                var n = c.Next;

                if(c.Value.IsNullOrEmpty() || ("." == c.Value))
                {
                    r.Remove(c);
                }
                else if(".." == c.Value)
                {
                    if(null == c.Previous)
                    {
                        break;
                    }

                    r.Remove(c.Previous);
                    r.Remove(c);
                }
                else if(PathDelimiters.Contains(c.Value[0]))
                {
                    // Special case a segment with a root directory marker
                    switch(GetPathFormat(r.First?.Value))
                    {
                        case PathFormat.Unc:
                        case PathFormat.Uri:

                            // Remove everything but the root segment
                            if(c.Value.Length > 1)
                            {
                                c.Value = c.Value.Remove(0, 1);
                                r.RemoveBetween(r.First, c);
                            }
                            else
                            {
                                r.RemoveBetween(r.First, c.Next);
                            }

                            break;

                        default:

                            // Remove everything
                            if(c != r.First)
                            {
                                if(c.Value.Length > 1)
                                {
                                    c.Value = c.Value.Remove(0, 1);
                                    r.RemoveBefore(c);
                                }
                                else
                                {
                                    r.RemoveBefore(c.Next);
                                }
                            }

                            break;
                    }
                }

                c = n;
            }

            return r;
        }

        public static IEnumerable<string> SplitPath(string path, StringSplitOptions options = default)
        {
            return path.Split(PathDelimiters, options);
        }

        public static IEnumerable<string> SplitPath(IEnumerable<string> paths, StringSplitOptions options = default)
        {
            return paths.SelectMany(path => SplitPath(path, options));
        }

        public static Path ToPath(object obj)
        {
            if(obj is Path path)
            {
                return path;
            }

            if(obj is FileSystemInfo info)
            {
                return new Path(info);
            }

            if(obj is Uri uri)
            {
                return new Path(uri);
            }

            if(obj is string str)
            {
                return new Path(str);
            }

            return null;
        }
    }
}
