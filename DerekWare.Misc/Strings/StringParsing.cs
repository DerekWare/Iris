using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DerekWare.Collections;

namespace DerekWare.Strings
{
    public enum StringMatchAlgorithms
    {
        /// <summary>
        ///     https://en.wikipedia.org/wiki/Levenshtein_distance
        /// </summary>
        LevenshteinDistance,

        /// <summary>
        ///     https://en.wikipedia.org/wiki/Hamming_distance
        /// </summary>
        ModifiedHammingDistance
    }

    [Flags]
    public enum StringSplitOptions
    {
        None = 0,

        /// <summary>
        ///     The return value does not include array elements that contain an empty string
        /// </summary>
        RemoveEmptyEntries = 1 << 0,

        /// <summary>
        ///     Ignore case when comparing separators
        /// </summary>
        IgnoreCase = 1 << 1,

        /// <summary>
        ///     Ignore splitters within quotation marks
        /// </summary>
        RespectQuotes = 1 << 2,

        /// <summary>
        ///     Preserve the markers after the split
        /// </summary>
        PreserveMarkers = 1 << 3
    }

    public class StringBlockMarker : IEnumerable<char>
    {
        public char End;
        public char Start;

        public StringBlockMarker()
        {
        }

        public StringBlockMarker(char marker)
            : this(marker, marker)
        {
        }

        public StringBlockMarker(char start, char end)
        {
            Start = start;
            End = end;
        }

        public override string ToString()
        {
            return $"{Start}{End}";
        }

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEnumerable<char>

        public IEnumerator<char> GetEnumerator()
        {
            return new List<char> { Start, End }.GetEnumerator();
        }

        #endregion
    }

    public static partial class StringParsing
    {
        public static readonly ValueMap<StringComparison, StringComparer> StringComparerMap = new ValueMap<StringComparison, StringComparer>
        {
            { StringComparison.CurrentCulture, StringComparer.CurrentCulture },
            { StringComparison.CurrentCultureIgnoreCase, StringComparer.CurrentCultureIgnoreCase },
            { StringComparison.InvariantCulture, StringComparer.InvariantCulture },
            { StringComparison.InvariantCultureIgnoreCase, StringComparer.InvariantCultureIgnoreCase },
            { StringComparison.Ordinal, StringComparer.Ordinal },
            { StringComparison.OrdinalIgnoreCase, StringComparer.OrdinalIgnoreCase }
        };

        public static bool Contains(this string value, string search, StringComparison comparison = StringComparison.Ordinal)
        {
            return value?.IndexOf(search, comparison) >= 0;
        }

        public static bool ContainsAny(this string item, char[] values, CharacterComparer comparer = null)
        {
            if(item.IsNullOrEmpty())
            {
                return false;
            }

            comparer = comparer ?? CharacterComparer.Default;
            return values.Any(value => item.Contains(value, comparer));
        }

        public static bool ContainsAny(this string item, IEnumerable<string> values, StringComparison comparison = StringComparison.Ordinal)
        {
            return !item.IsNullOrEmpty() && values.Any(value => item.Contains(value, comparison));
        }

        public static Dictionary<char, int> CountCharacters(this string value, bool ignoreCase, IReadOnlyList<char> characters)
        {
            return CountCharacters(value.AsEnumerable(), ignoreCase, characters);
        }

        public static Dictionary<char, int> CountCharacters(this IEnumerable<string> values, bool ignoreCase, IReadOnlyList<char> characters)
        {
            var comparer = new CharacterComparer(ignoreCase);
            var result = new Dictionary<char, int>(comparer);
            var empty = characters.IsNullOrEmpty();

            foreach(var c in characters)
            {
                result[c] = 0;
            }

            var found = from value in values.SafeEmpty().WhereNotNull()
                        from c in value
                        where empty || characters.Contains(c, comparer)
                        select c;

            foreach(var c in found)
            {
                result[c] += 1;
            }

            return result;
        }

        public static bool EndsWith(this string item, char[] values)
        {
            return !item.IsNullOrEmpty() && values.Any(value => item[item.Length - 1] == value);
        }

        public static bool EndsWith(this string item, IEnumerable<string> values, StringComparison comparison = StringComparison.Ordinal)
        {
            return !item.IsNullOrEmpty() && values.Any(value => item.EndsWith(value, comparison));
        }

        public static bool EqualTo(this string item, string value, StringComparison comparison = StringComparison.Ordinal)
        {
            return string.Equals(item, value, comparison);
        }

        public static bool EqualTo(this string item, IEnumerable<string> values, StringComparison comparison = StringComparison.Ordinal)
        {
            return values.Any(value => EqualTo(item, value, comparison));
        }

        public static string Format(this string value, params object[] args)
        {
            return value.IsNullOrEmpty() ? string.Empty : args.IsNullOrEmpty() ? value : string.Format(value, args);
        }

        public static KeyValuePair<char, int> GetPrimaryCharacter(this string value, bool ignoreCase, params char[] characters)
        {
            return GetPrimaryCharacter(value, ignoreCase, (IReadOnlyList<char>)characters);
        }

        public static KeyValuePair<char, int> GetPrimaryCharacter(this IEnumerable<string> values, bool ignoreCase, params char[] characters)
        {
            return GetPrimaryCharacter(values, ignoreCase, (IReadOnlyList<char>)characters);
        }

        public static KeyValuePair<char, int> GetPrimaryCharacter(this string value, bool ignoreCase, IReadOnlyList<char> characters)
        {
            return GetPrimaryCharacter(value.AsEnumerable(), ignoreCase, characters);
        }

        public static KeyValuePair<char, int> GetPrimaryCharacter(this IEnumerable<string> values, bool ignoreCase, IReadOnlyList<char> characters)
        {
            return (from k in CountCharacters(values, ignoreCase, characters)
                    orderby k.Value descending, k.Key
                    select k).FirstOrDefault();
        }

        public static string IfNullOrEmpty(this string value, string other)
        {
            return string.IsNullOrEmpty(value) ? other : value;
        }

        public static int IndexOf(this string value, char search, int offset = 0, bool ignoreCase = false)
        {
            var comparer = new CharacterComparer(ignoreCase);
            var length = value?.Length ?? 0;

            while(offset < length)
            {
                if(comparer.Equals(value[offset], search))
                {
                    return offset;
                }

                ++offset;
            }

            return -1;
        }

        public static KeyValuePair<char, int>? IndexOfAny(this string value, char[] search, int offset = 0, bool ignoreCase = false)
        {
            return (from i in search
                    let index = value.IndexOf(i, offset, ignoreCase)
                    where index >= offset
                    orderby index
                    select new KeyValuePair<char, int>?(new KeyValuePair<char, int>(i, index))).FirstOrDefault();
        }

        public static KeyValuePair<string, int>? IndexOfAny(
            this string value,
            IEnumerable<string> search,
            int offset = 0,
            StringComparison comparison = StringComparison.Ordinal)
        {
            return (from i in search
                    let index = value.IndexOf(i, offset, comparison)
                    where index >= offset
                    orderby index
                    select new KeyValuePair<string, int>?(new KeyValuePair<string, int>(i, index))).FirstOrDefault();
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static string RemoveBlocks(this string @this, IReadOnlyList<StringBlockMarker> markers, StringSplitOptions options = default)
        {
            return SplitBlocks(@this, markers, options | StringSplitOptions.RemoveEmptyEntries).WhereNull(b => b.Key).Select(b => b.Value).JoinWords();
        }

        public static string SafeToString(this object value)
        {
            return value?.ToString() ?? string.Empty;
        }

        public static string SafeToString(this Guid value, string format = "B")
        {
            return value.ToString(format).ToUpper();
        }

        public static List<string> Split(this string value, params char[] separator)
        {
            return value.Split((IEnumerable<char>)separator);
        }

        public static List<string> Split(this string value, IEnumerable<char> separators, StringSplitOptions options = default)
        {
            return value.Split(separators, int.MaxValue, options);
        }

        public static List<string> Split(this string value, IEnumerable<char> separators, int maxCount, StringSplitOptions options = default)
        {
            return value.Split(separators.Contains, maxCount, options);
        }

        public static List<string> Split(this string value, Func<char, bool> selector, StringSplitOptions options = default)
        {
            return value.Split(selector, int.MaxValue, options);
        }

        /// <summary>
        ///     Main Split routine for character-based splits.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="selector"></param>
        /// <param name="maxCount"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static List<string> Split(this string value, Func<char, bool> selector, int maxCount, StringSplitOptions options = default)
        {
            var result = new List<string>();
            var offset = 0;
            var quote = false;

            value = value ?? "";

            while((offset < value.Length) && ((result.Count + 1) < maxCount))
            {
                var c = value[offset];

                if(options.HasFlag(StringSplitOptions.RespectQuotes) && ('\"' == c))
                {
                    quote = !quote;
                }

                if(quote)
                {
                    ++offset;
                    continue;
                }

                bool sep;

                if(options.HasFlag(StringSplitOptions.IgnoreCase))
                {
                    sep = selector(char.ToUpper(c)) || selector(char.ToLower(c));
                }
                else
                {
                    sep = selector(c);
                }

                if(!sep)
                {
                    ++offset;
                    continue;
                }

                if((offset > 0) || !options.HasFlag(StringSplitOptions.RemoveEmptyEntries) || options.HasFlag(StringSplitOptions.PreserveMarkers))
                {
                    result.Add(value.Substring(0, options.HasFlag(StringSplitOptions.PreserveMarkers) ? offset + 1 : offset));
                }

                value = value.Remove(0, offset + 1);
                offset = 0;
            }

            if(!value.IsNullOrEmpty())
            {
                result.Add(value);
            }

            return result;
        }

        public static List<string> Split(this string value, IEnumerable<string> separators, StringSplitOptions options = default)
        {
            return value.Split(separators, int.MaxValue, options);
        }

        public static List<string> Split(this string value, IEnumerable<string> separators, int maxCount, StringSplitOptions options = default)
        {
            separators = separators.OrderByDescending(i => i.Length).ToList();

            var result = new List<string>();

            while(!value.IsNullOrEmpty() && ((result.Count + 1) < maxCount))
            {
                var found = false;

                foreach(var i in separators)
                {
                    if(found = SplitWorker(ref value, i, options, result))
                    {
                        break;
                    }
                }

                if(!found)
                {
                    break;
                }
            }

            if(!value.IsNullOrEmpty())
            {
                result.Add(value);
            }

            return result;
        }

        public static List<string> SplitArguments(this string @this, IReadOnlyList<StringBlockMarker> markers = null)
        {
            // Split the string into blocks of single words or quoted text
            // TODO escaped characters
            var result = new List<string>();

            if(markers.IsNullOrEmpty())
            {
                markers = new[] { new StringBlockMarker('\"') };
            }

            foreach(var block in SplitBlocks(@this, markers, StringSplitOptions.RemoveEmptyEntries))
            {
                var text = block.Value;

                if(null != block.Key)
                {
                    // This was part of a quoted block, so return the whole thing
                    result.Add(text);
                }
                else
                {
                    // This isn't part of a quoted block, so split the words into separate arguments
                    result.AddRange(text.SplitWords());
                }
            }

            // Unescape characters
            for(var i = 0; i < result.Count; ++i)
            {
                foreach(var marker in markers)
                {
                    result[i] = result[i].Replace($"\\{marker.Start}", marker.Start.ToString()).Replace($"\\{marker.End}", marker.End.ToString());
                }
            }

            return result;
        }

        public static string[] SplitAt(this string value, int index, int length = 0)
        {
            return new[] { value.Substring(0, index), value.Substring(index + length, value.Length - index - length) };
        }

        public static IEnumerable<KeyValuePair<StringBlockMarker, string>> SplitBlocks(
            this string @this,
            IReadOnlyList<StringBlockMarker> markers,
            StringSplitOptions options = default)
        {
            // TODO escaped characters
            // TODO blocks within blocks

            StringBlockMarker currentMarker = null;
            var quote = false;
            string value = null;

            foreach(var c in @this.SafeEmpty())
            {
                // Check for quotes
                if(options.HasFlag(StringSplitOptions.RespectQuotes) && ('"' == c))
                {
                    quote = !quote;
                }

                if(quote)
                {
                    value += c;
                    continue;
                }

                // Check for block end
                if(currentMarker?.End == c)
                {
                    if(options.HasFlag(StringSplitOptions.PreserveMarkers))
                    {
                        value += c;
                    }

                    value = value?.Trim();

                    if(!options.HasFlag(StringSplitOptions.RemoveEmptyEntries) || !value.IsNullOrEmpty())
                    {
                        yield return KeyValuePair.Create(currentMarker, value);
                    }

                    currentMarker = null;
                    value = null;

                    continue;
                }

                // Check for block start
                if((null == currentMarker) && (null != (currentMarker = markers.FirstEquals(v => v.Start, c))))
                {
                    value = value?.Trim();

                    if(!options.HasFlag(StringSplitOptions.RemoveEmptyEntries) || !value.IsNullOrEmpty())
                    {
                        yield return KeyValuePair.Create((StringBlockMarker)null, value);
                    }

                    value = null;

                    if(options.HasFlag(StringSplitOptions.PreserveMarkers))
                    {
                        value += c;
                    }

                    continue;
                }

                // Accumulate the string
                value += c;
            }

            value = value?.Trim();

            if(!options.HasFlag(StringSplitOptions.RemoveEmptyEntries) || !value.IsNullOrEmpty())
            {
                yield return KeyValuePair.Create(currentMarker, value);
            }
        }

        public static List<string> SplitLines(this string value, StringSplitOptions options = default)
        {
            return Split(value, new[] { "\r\n", "\n\r", "\r", "\n" }, options);
        }

        public static List<string> SplitParagraph(this string lines, int lineLength)
        {
            return lines.AsEnumerable().SplitParagraph(lineLength);
        }

        public static List<string> SplitParagraph(this IEnumerable<string> lines, int lineLength)
        {
            var result = new List<string>();
            lines = lines.SelectMany(v => v.SplitLines());

            foreach(var line in lines)
            {
                var value = "";

                if(line.Length <= 0)
                {
                    result.Add(value);
                    continue;
                }

                foreach(var word in line.SplitWords())
                {
                    if(!string.IsNullOrEmpty(value))
                    {
                        if((value.Length + word.Length + 1) >= lineLength)
                        {
                            result.Add(value);
                            value = "";
                        }
                        else
                        {
                            value += " ";
                        }
                    }

                    value += word;
                }

                if(!string.IsNullOrEmpty(value))
                {
                    result.Add(value);
                }
            }

            return result;
        }

        public static List<string> SplitWords(this string value, StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
        {
            return value.Split(char.IsWhiteSpace, options);
        }

        public static bool StartsWith(this string item, IEnumerable<string> values, StringComparison comparison = StringComparison.Ordinal)
        {
            return !item.IsNullOrEmpty() && values.Any(value => item.StartsWith(value, comparison));
        }

        public static string ToParagraph(this string lines, int lineLength)
        {
            return lines.AsEnumerable().ToParagraph(lineLength);
        }

        public static string ToParagraph(this IEnumerable<string> lines, int lineLength)
        {
            return SplitParagraph(lines, lineLength).Join(Environment.NewLine);
        }

        static bool SplitWorker(ref string value, string splitter, StringSplitOptions options, List<string> result)
        {
            // TODO RespectQuotes
            var offset = value.IndexOf(splitter,
                                       options.HasFlag(StringSplitOptions.IgnoreCase) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

            if(offset < 0)
            {
                return false;
            }

            if((offset > 0) || !options.HasFlag(StringSplitOptions.RemoveEmptyEntries) || options.HasFlag(StringSplitOptions.PreserveMarkers))
            {
                result.Add(value.Substring(0, options.HasFlag(StringSplitOptions.PreserveMarkers) ? offset + splitter.Length : offset));
            }

            value = value.Remove(0, offset + splitter.Length);
            return true;
        }
    }
}
