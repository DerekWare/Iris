using System;
using System.Collections.Generic;
using System.Linq;
using DerekWare.Strings;
using StringSplitOptions = DerekWare.Strings.StringSplitOptions;

namespace DerekWare
{
    public static class CommandLine
    {
        public delegate bool OptionFoundDelegate(string key, string value);

        #region Conversion

        /// <summary>
        ///     Given a list of command line arguments, extracts the options and removes
        ///     them from the list. If any Option has its Found delegate set, the delegate
        ///     will be called.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="args"></param>
        /// <param name="ignoreInvalid"></param>
        /// <returns>The remaining arguments that are not options.</returns>
        public static List<string> Parse(this IReadOnlyCollection<Option> options, IEnumerable<string> args, bool ignoreInvalid = false)
        {
            var remaining = new List<string>();

            foreach(var arg in args)
            {
                if(string.IsNullOrEmpty(arg))
                {
                    continue;
                }

                if(!Option.PrefixCharacters.Contains(arg[0]))
                {
                    remaining.Add(arg);
                    continue;
                }

                var parts = arg.Remove(0, 1).Split(Option.PrefixSeparators, 2, StringSplitOptions.RemoveEmptyEntries).Trim().ToArray();
                var values = parts.Length > 1 ? parts[1].Split(Option.ValueSeparators, StringSplitOptions.RemoveEmptyEntries).Trim().ToArray() : new string[1];
                var option = options.FirstOrDefault(v => parts[0].EqualTo(v.Keys, StringComparison.OrdinalIgnoreCase));
                var valid = null != option;

                if(valid && (null != option.Found))
                {
                    valid = values.Select(value => option.Found(parts[0], value)).All(result => result);
                }

                if(!valid && !ignoreInvalid)
                {
                    throw new ArgumentException($"Invalid option: {parts[0]}");
                }
            }

            return remaining;
        }

        #endregion

        /// <summary>
        ///     Creates a usage string from a list of all available options.
        /// </summary>
        /// <param name="options">All available command line options</param>
        /// <param name="alignment">The number of padding spaces to add to the right of the command line switch</param>
        /// <param name="maxLineLength">The maximum line length to allow when formatting the usage text</param>
        /// <returns>An enumerable containing the lines of the usage text</returns>
        public static List<string> GetUsage(IReadOnlyCollection<Option> options, int alignment, int maxLineLength)
        {
            var result = new List<string>();

            alignment = Math.Max(alignment, options.Select(i => i.Key.Length).Max() + 3);

            foreach(var option in options.OrderBy(v => v.Key, StringComparer.OrdinalIgnoreCase))
            {
                var value = (Option.PrefixCharacters[0] + option.Key).PadRight(alignment);

                using(var description = option.Description.SplitParagraph(maxLineLength - alignment).GetEnumerator())
                {
                    if(description.MoveNext())
                    {
                        result.Add(value + description.Current);
                    }

                    while(description.MoveNext())
                    {
                        result.Add(description.Current.Indent(alignment));
                    }
                }
            }

            return result;
        }

        public class Option
        {
            public static readonly char[] PrefixCharacters = { '/', '-' };
            public static readonly char[] PrefixSeparators = { ':' };
            public static readonly char[] ValueSeparators = { ',', ';' };

            public string Description;
            public OptionFoundDelegate Found;
            public string[] Keys;

            public string Key { get => Keys.Join('|'); set => Keys = value.Split('|'); }
        }
    }
}
