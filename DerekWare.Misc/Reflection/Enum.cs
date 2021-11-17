using System;
using System.Collections.Generic;
using System.Linq;
using DerekWare.Collections;
using DerekWare.Strings;

namespace DerekWare.Reflection
{
    // TODO I haven't looked at this class in a very long time, but it doesn't seem to be using the
    // Name attribute and it should probably use the EnumMember attribute as well.
    public static class Enum
    {
        #region Conversion

        /// <summary>
        ///     This version of Enum.Parse will use all possible names for the enum members, including the Name attribute.
        /// </summary>
        public static TEnum Parse<TEnum>(string text, bool ignoreCase)
            where TEnum : struct
        {
            if(!TryParse(text, ignoreCase, out TEnum result))
            {
                throw new FormatException();
            }

            return result;
        }

        /// <summary>
        ///     This version of Enum.Parse will use all possible names for the enum members, including the Name attribute.
        /// </summary>
        public static TEnum Parse<TEnum>(string text)
            where TEnum : struct
        {
            if(!TryParse(text, out TEnum result))
            {
                throw new FormatException();
            }

            return result;
        }

        #endregion

        public static IEnumerable<KeyValuePair<string, TEnum>> GetMembers<TEnum>()
            where TEnum : struct
        {
            if(!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException($"{typeof(TEnum)} is not an enum");
            }

            return new Reflector(typeof(TEnum)).GetFieldValues<TEnum>();
        }

        public static Dictionary<string, TEnum> GetMembers<TEnum>(IEqualityComparer<string> comparer)
            where TEnum : struct
        {
            return GetMembers<TEnum>().ToDictionary(comparer);
        }

        public static IEnumerable<string> GetNames<TEnum>()
            where TEnum : struct
        {
            return GetMembers<TEnum>().Select(v => v.Key).Distinct().OrderBy(v => v);
        }

        public static T GetNextEnumValue<T>(this T @this, int delta = 1)
        {
            return ((T[])System.Enum.GetValues(typeof(T))).GetNextValue(@this, delta);
        }

        public static IEnumerable<TEnum> GetValues<TEnum>()
            where TEnum : struct
        {
            return GetMembers<TEnum>().Select(v => v.Value).Distinct().OrderBy(v => v);
        }

        /// <summary>
        ///     This version of Enum.TryParse will use all possible names for the enum members, including the Name attribute.
        /// </summary>
        public static bool TryParse<TEnum>(string text, bool ignoreCase, out TEnum result)
            where TEnum : struct
        {
            var map = GetMembers<TEnum>(ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
            var parts = text.Split(",".ToCharArray()).Trim().ToArray();

            result = default;

            for(var i = 0; i < parts.Length; ++i)
            {
                if(!map.TryGetValue(parts[i], out var value))
                {
                    return false;
                }

                parts[i] = value.ToString();
            }

            text = parts.Join(", ");

            return System.Enum.TryParse(text, ignoreCase, out result);
        }

        /// <summary>
        ///     This version of Enum.TryParse will use all possible names for the enum members, including the Name attribute.
        /// </summary>
        public static bool TryParse<TEnum>(string text, out TEnum result)
            where TEnum : struct
        {
            return TryParse(text, false, out result);
        }
    }
}
