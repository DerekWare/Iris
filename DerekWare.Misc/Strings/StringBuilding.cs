using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DerekWare.Collections;

namespace DerekWare.Strings
{
    public static class StringBuilding
    {
        public static string Indent(this string value, int count)
        {
            return Indent(value, count, ' ');
        }

        public static string Indent(this string value, int count, char paddingChar)
        {
            return value.PadLeft(value.Length + count, paddingChar);
        }

        public static string Join(this IEnumerable<string> values, char separator)
        {
            return Join(values, separator.ToString());
        }

        public static string Join(this IEnumerable<string> values, string separator)
        {
            return string.Join(separator, values);
        }

        public static string Join<T>(this IEnumerable<T> values, char separator)
        {
            return Join(values, separator.ToString());
        }

        public static string Join<T>(this IEnumerable<T> values, string separator)
        {
            return string.Join(separator, values);
        }

        public static string Join(this IEnumerable values, char separator)
        {
            return Join(values, separator.ToString());
        }

        public static string Join(this IEnumerable values, string separator)
        {
            return string.Join(separator, values.Cast<object>());
        }

        public static string JoinLines(this IEnumerable<string> values)
        {
            return JoinLines(values, Environment.NewLine);
        }

        public static string JoinLines(this IEnumerable<string> values, string separator)
        {
            return Join(values.SelectMany(part => part.SplitLines()), separator);
        }

        public static string JoinWords(this IEnumerable<string> values)
        {
            return JoinWords(values, " ");
        }

        public static string JoinWords(this IEnumerable<string> values, string separator)
        {
            return Join(values.SelectMany(part => part.SplitWords()), separator);
        }

        public static string Quote(this string value)
        {
            return Surround(value, "\"");
        }

        public static string Remove(this string value, string remove, StringComparison comparison = StringComparison.Ordinal)
        {
            return Replace(value, remove, null, comparison);
        }

        public static string Remove(this string value, IReadOnlyList<string> remove, StringComparison comparison = StringComparison.Ordinal)
        {
            return Replace(value, remove, null, comparison);
        }

        public static string Remove(this string value, params char[] remove)
        {
            return value.Where(c => !remove.Contains(c));
        }

        public static bool Replace(ref string value, string source, string target, StringComparison comparison = StringComparison.Ordinal)
        {
            var modified = false;

            while(true)
            {
                var index = value.IndexOf(source, 0, comparison);

                if(index < 0)
                {
                    break;
                }

                var split = value.SplitAt(index, source.Length);
                value = split[0] + target + split[1];
                modified = true;
            }

            return modified;
        }

        public static string Replace(this string value, string source, string target, StringComparison comparison = StringComparison.Ordinal)
        {
            Replace(ref value, source, target, comparison);
            return value;
        }

        public static string Replace(this string value, IReadOnlyList<string> sources, string target, StringComparison comparison = StringComparison.Ordinal)
        {
            for(var i = 0; i < sources.Count;)
            {
                i = Replace(ref value, sources[i], target, comparison) ? 0 : i + 1;
            }

            return value;
        }

        public static string Replace(
            this string value,
            IReadOnlyList<KeyValuePair<string, string>> replace,
            StringComparison comparison = StringComparison.Ordinal)
        {
            for(var i = 0; i < replace.Count;)
            {
                i = Replace(ref value, replace[i].Key, replace[i].Value, comparison) ? 0 : i + 1;
            }

            return value;
        }

        public static string Replace(this string value, char source, char target, bool ignoreCase = false)
        {
            var comparer = new CharacterComparer(ignoreCase);
            var result = value.ToCharArray();

            for(var i = 0; i < result.Length; ++i)
            {
                if(comparer.Equals(result[i], source))
                {
                    result[i] = target;
                }
            }

            return new string(result);
        }

        public static string Replace(this string value, char[] sources, char target, bool ignoreCase = false)
        {
            var comparer = new CharacterComparer(ignoreCase);
            var result = value.ToCharArray();

            for(var i = 0; i < result.Length; ++i)
            {
                if(sources.Any(c => comparer.Equals(result[i], c)))
                {
                    result[i] = target;
                }
            }

            return new string(result);
        }

        public static string Surround(this string value, string start)
        {
            return Surround(value, start, start);
        }

        public static string Surround(this string value, string start, string end)
        {
            if(!value.StartsWith(start))
            {
                value = start + value;
            }

            if(!value.EndsWith(end))
            {
                value = value + end;
            }

            return value;
        }

        public static IEnumerable<string> Trim(this IEnumerable<string> items)
        {
            return items.Select(v => v.SafeToString().Trim());
        }

        public static IEnumerable<string> Trim(this IEnumerable<string> items, char trim)
        {
            return items.Select(v => v.SafeToString().Trim(trim));
        }

        public static IEnumerable<string> Trim(this IEnumerable<string> items, string trim)
        {
            return items.Select(v => v.SafeToString().Trim(trim));
        }

        public static IEnumerable<string> Trim(
            this IEnumerable<string> value,
            IReadOnlyList<string> trim,
            StringComparison comparison = StringComparison.Ordinal)
        {
            return value.Select(v => v.SafeToString().Trim(trim, comparison));
        }

        public static string Trim(this string value, string remove, StringComparison comparison = StringComparison.Ordinal)
        {
            return value.TrimStart(remove, comparison).TrimEnd(remove, comparison);
        }

        public static string Trim(this string value, IReadOnlyList<string> remove, StringComparison comparison = StringComparison.Ordinal)
        {
            return value.TrimStart(remove, comparison).TrimEnd(remove, comparison);
        }

        public static IEnumerable<string> TrimEnd(this IEnumerable<string> items)
        {
            return items.Select(v => v.SafeToString().TrimEnd());
        }

        public static IEnumerable<string> TrimEnd(this IEnumerable<string> items, char trim)
        {
            return items.Select(v => v.SafeToString().TrimEnd(trim));
        }

        public static IEnumerable<string> TrimEnd(this IEnumerable<string> items, string trim)
        {
            return items.Select(v => v.SafeToString().TrimEnd(trim));
        }

        public static IEnumerable<string> TrimEnd(
            this IEnumerable<string> value,
            IReadOnlyList<string> trim,
            StringComparison comparison = StringComparison.Ordinal)
        {
            return value.Select(v => v.SafeToString().TrimEnd(trim, comparison));
        }

        public static string TrimEnd(this string value, string remove, StringComparison comparison = StringComparison.Ordinal)
        {
            var length = remove.Length;

            while(value?.EndsWith(remove, comparison) ?? false)
            {
                value = value.Remove(value.Length - length);
            }

            return value;
        }

        public static string TrimEnd(this string value, IReadOnlyList<string> remove, StringComparison comparison = StringComparison.Ordinal)
        {
            while(!value.IsNullOrEmpty())
            {
                var prev = value;

                foreach(var i in remove)
                {
                    value = value.TrimEnd(i, comparison);
                }

                if(value == prev)
                {
                    break;
                }
            }

            return value;
        }

        public static IEnumerable<string> TrimStart(this IEnumerable<string> items)
        {
            return items.Select(v => v.SafeToString().TrimStart());
        }

        public static IEnumerable<string> TrimStart(this IEnumerable<string> items, char trim)
        {
            return items.Select(v => v.SafeToString().TrimStart(trim));
        }

        public static IEnumerable<string> TrimStart(this IEnumerable<string> items, string trim)
        {
            return items.Select(v => v.SafeToString().TrimStart(trim));
        }

        public static IEnumerable<string> TrimStart(
            this IEnumerable<string> value,
            IReadOnlyList<string> trim,
            StringComparison comparison = StringComparison.Ordinal)
        {
            return value.Select(v => v.SafeToString().TrimStart(trim, comparison));
        }

        public static string TrimStart(this string value, string remove, StringComparison comparison = StringComparison.Ordinal)
        {
            var length = remove.Length;

            while(value?.StartsWith(remove, comparison) ?? false)
            {
                value = value.Remove(0, length);
            }

            return value;
        }

        public static string TrimStart(this string value, IReadOnlyList<string> remove, StringComparison comparison = StringComparison.Ordinal)
        {
            while(!value.IsNullOrEmpty())
            {
                var prev = value;

                foreach(var i in remove)
                {
                    value = value.TrimStart(i, comparison);
                }

                if(value == prev)
                {
                    break;
                }
            }

            return value;
        }

        public static IEnumerable<string> TrimWords(this IEnumerable<string> value)
        {
            return value.Select(v => v.SafeToString().TrimWords());
        }

        public static string TrimWords(this string value)
        {
            return JoinWords(value.AsEnumerable());
        }
    }
}
