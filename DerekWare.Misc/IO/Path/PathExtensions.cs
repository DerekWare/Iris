using System;
using System.Collections.Generic;
using System.Linq;

namespace DerekWare.IO
{
    public static partial class PathExtensions
    {
        public static bool IsNullOrEmpty(this Path @this)
        {
            return @this?.IsEmpty != false;
        }

        public static IEnumerable<Path> WhereNotNull(this IEnumerable<Path> @this)
        {
            return @this.Where(v => !IsNullOrEmpty(v));
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> @this, Func<T, Path> selector)
        {
            return @this.Where(v => !selector(v).IsNullOrEmpty());
        }

        public static IEnumerable<Path> WhereNull(this IEnumerable<Path> @this)
        {
            return @this.Where(v => v.IsNullOrEmpty());
        }

        public static IEnumerable<T> WhereNull<T>(this IEnumerable<T> @this, Func<T, Path> selector)
        {
            return @this.Where(v => selector(v).IsNullOrEmpty());
        }
    }
}
