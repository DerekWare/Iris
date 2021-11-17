using System;
using System.Collections.Generic;
using System.Linq;
using DerekWare.Collections;

namespace DerekWare.Strings
{
    public static partial class StringParsing
    {
        public static readonly IReadOnlyDictionary<StringMatchAlgorithms, Func<string, string, bool, int>> StringMatchAlgorithmMap =
            new Dictionary<StringMatchAlgorithms, Func<string, string, bool, int>>
            {
                { StringMatchAlgorithms.LevenshteinDistance, GetLevenshteinDistance },
                { StringMatchAlgorithms.ModifiedHammingDistance, GetModifiedHammingDistance }
            };

        /// <summary>
        ///     Calculates the Levenshtein distance between two strings.
        /// 
        ///     https://en.wikipedia.org/wiki/Levenshtein_distance
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="ignoreCase"></param>
        /// <returns>
        ///     The count of edits required to make the two strings match.
        /// 
        ///     For example, the Levenshtein distance between "kitten" and "sitting" is 3, since the following three edits change
        ///     one into the other, and there is no way to do it with fewer than three edits:
        /// 
        ///     - kitten ? sitten (substitution of "s" for "k")
        ///     - sitten ? sittin (substitution of "i" for "e")
        ///     - sittin ? sitting (insertion of "g" at the end).
        /// 
        ///     The Levenshtein distance has several simple upper and lower bounds.These include:
        /// 
        ///     - It is at least the difference of the sizes of the two strings.
        ///     - It is at most the length of the longer string.
        ///     - It is zero if and only if the strings are equal.
        ///     - If the strings are the same size, the Hamming distance is an upper bound on the Levenshtein distance.
        /// </returns>
        public static int GetLevenshteinDistance(string x, string y, bool ignoreCase)
        {
            var m = x?.Length ?? 0;
            var n = y?.Length ?? 0;

            if(m <= 0)
            {
                return n;
            }

            if(n <= 0)
            {
                return m;
            }

            var c = new CharacterComparer(ignoreCase);
            var d = new int[m + 1, n + 1];

            for(var i = 0; i <= m; i++)
            {
                d[i, 0] = i;
            }

            for(var j = 0; j <= n; j++)
            {
                d[0, j] = j;
            }

            for(var j = 1; j <= n; j++)
            {
                for(var i = 1; i <= m; i++)
                {
                    if(c.Equals(x[i - 1], y[j - 1]))
                    {
                        d[i, j] = d[i - 1, j - 1]; // no operation
                    }
                    else
                    {
                        d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, // a deletion
                                                    d[i, j - 1] + 1), // an insertion
                                           d[i - 1, j - 1] + 1 // a substitution
                        );
                    }
                }
            }

            return d[m, n];
        }

        /// <summary>
        ///     Calculates the Hamming distance between two strings. Unlike the pure Hamming algorithm, this algorithm allows for
        ///     strings of different lengths and factors that into the result.
        /// 
        ///     https://en.wikipedia.org/wiki/Levenshtein_distance
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="ignoreCase"></param>
        /// <returns>
        ///     The count of edits required to make the two strings match.
        /// 
        ///     The Hamming distance between:
        ///     - "kaROLin" and "kaTHRin" is 3.
        ///     - "kArOLin" and "kErSTin" is 3.
        ///     - 1011101 and 1001001 is 2.
        ///     - 2173896 and 2233796 is 3.
        /// </returns>
        public static int GetModifiedHammingDistance(string x, string y, bool ignoreCase)
        {
            var d = new Dictionary<char, int>(new CharacterComparer(ignoreCase));
            var m = x?.Length ?? 0;
            var n = y?.Length ?? 0;
            var l = Math.Max(m, n);

            for(var i = 0; i < l; ++i)
            {
                if(i < m)
                {
                    d.SetValue(x[i], d.GetValue(x[i]) + 1);
                }

                if(i < n)
                {
                    d.SetValue(y[i], d.GetValue(y[i]) - 1);
                }
            }

            return d.Sum(k => Math.Abs(k.Value));
        }

        /// <summary>
        ///     Calculates the difference between two strings.
        /// </summary>
        /// <returns>A ratio comparing the number of matches to the maximum length of the two strings.</returns>
        public static Ratio Matches(this string x, string y)
        {
            return Matches(x, y, false, default);
        }

        /// <summary>
        ///     Calculates the difference between two strings.
        /// </summary>
        /// <returns>A ratio comparing the number of matches to the maximum length of the two strings.</returns>
        public static Ratio Matches(this string x, string y, bool ignoreCase)
        {
            return Matches(x, y, ignoreCase, default);
        }

        /// <summary>
        ///     Calculates the difference between two strings.
        /// </summary>
        /// <returns>A ratio comparing the number of matches to the maximum length of the two strings.</returns>
        public static Ratio Matches(this string x, string y, StringMatchAlgorithms algorithm)
        {
            return Matches(x, y, false, algorithm);
        }

        /// <summary>
        ///     Calculates the difference between two strings.
        /// </summary>
        /// <returns>A ratio comparing the number of matches to the maximum length of the two strings.</returns>
        public static Ratio Matches(this string x, string y, bool ignoreCase, StringMatchAlgorithms algorithm)
        {
            var d = StringMatchAlgorithmMap[algorithm](x, y, ignoreCase);
            var l = Math.Max(x?.Length ?? 0, y?.Length ?? 0);
            return new Ratio(l - d, l);
        }
    }
}
