using System;
using System.IO;

namespace DerekWare.IO
{
    public partial class Path
    {
        #region Equality

        public static bool operator ==(Path x, Path y)
        {
            return Equals(x, y);
        }

        public static bool operator ==(Path x, object y)
        {
            return Equals(x, y);
        }

        public static bool operator ==(object x, Path y)
        {
            return Equals(x, y);
        }

        public static bool operator !=(Path x, Path y)
        {
            return !Equals(x, y);
        }

        public static bool operator !=(Path x, object y)
        {
            return !Equals(x, y);
        }

        public static bool operator !=(object x, Path y)
        {
            return !Equals(x, y);
        }

        #endregion

        public static Path operator +(Path x, Path y)
        {
            return new Path(x, y);
        }

        public static Path operator +(Path x, FileSystemInfo y)
        {
            return new Path(x, new Path(y));
        }

        public static Path operator +(Path x, Uri y)
        {
            return new Path(x, new Path(y));
        }

        public static Path operator +(Path x, string y)
        {
            return new Path(x, new Path(y));
        }

        public static bool operator >(Path x, Path y)
        {
            return Compare(x, y) > 0;
        }

        public static bool operator >=(Path x, Path y)
        {
            return Compare(x, y) >= 0;
        }

        public static implicit operator string(Path obj)
        {
            return obj?.ToString();
        }

        public static implicit operator Uri(Path obj)
        {
            return obj?.ToUri();
        }

        public static bool operator <(Path x, Path y)
        {
            return Compare(x, y) < 0;
        }

        public static bool operator <=(Path x, Path y)
        {
            return Compare(x, y) <= 0;
        }
    }
}
