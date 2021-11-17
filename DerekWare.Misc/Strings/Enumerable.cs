using System;
using System.Collections.Generic;
using System.Linq;

namespace DerekWare.Collections
{
    public static partial class Enumerable
    {
        public static IEnumerable<string> AsEnumerable(this string item, bool removeNull = false)
        {
            return removeNull && item.IsNullOrEmpty() ? System.Linq.Enumerable.Empty<string>() : new[] { item };
        }

        public static string FirstEquals(this IEnumerable<string> items, string value, StringComparison comparison = StringComparison.Ordinal)
        {
            return items.WhereEquals(value, comparison).FirstOrDefault();
        }

        public static string FirstNotEquals(this IEnumerable<string> items, string value, StringComparison comparison = StringComparison.Ordinal)
        {
            return items.WhereNotEquals(value, comparison).FirstOrDefault();
        }

        public static string FirstNotNull(this IEnumerable<string> items)
        {
            return items.WhereNotNull().FirstOrDefault();
        }

        public static string FirstNotNull(params string[] items)
        {
            return FirstNotNull((IEnumerable<string>)items);
        }

        public static string FirstNull(this IEnumerable<string> items)
        {
            return items.WhereNull().FirstOrDefault();
        }

        public static string FirstNull(params string[] items)
        {
            return FirstNull((IEnumerable<string>)items);
        }

        public static string Select(this string value, Func<char, char> selector)
        {
            return new string(System.Linq.Enumerable.Select(value, selector).ToArray());
        }

        public static string Where(this string value, Func<char, bool> selector)
        {
            return new string(System.Linq.Enumerable.Where(value, selector).ToArray());
        }

        public static IEnumerable<string> WhereEquals(this IEnumerable<string> items, string value, StringComparison comparison = StringComparison.Ordinal)
        {
            return items.Where(v => string.Equals(v, value, comparison));
        }

        public static string WhereNot(this string value, Func<char, bool> selector)
        {
            return new string(Where(value, c => !selector(c)).ToArray());
        }

        public static IEnumerable<string> WhereNotEquals(this IEnumerable<string> items, string value, StringComparison comparison = StringComparison.Ordinal)
        {
            return items.Where(v => !string.Equals(v, value, comparison));
        }

        public static IEnumerable<string> WhereNotNull(this IEnumerable<string> items)
        {
            return items.Where(v => !string.IsNullOrEmpty(v));
        }

        public static IEnumerable<string> WhereNull(this IEnumerable<string> items)
        {
            return items.Where(string.IsNullOrEmpty);
        }
    }
}
