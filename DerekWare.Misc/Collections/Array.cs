using System;
using System.Collections.Generic;

namespace DerekWare.Collections
{
    public static partial class Enumerable
    {
        /// <summary>
        ///     Replaces Array.Copy by preserving multidimensional array offsets.
        /// </summary>
        public static Array CopyTo(this Array src, Array dst)
        {
            if(ReferenceEquals(null, src) || ReferenceEquals(null, dst))
            {
                return dst;
            }

            if(src.Rank != dst.Rank)
            {
                throw new ArgumentOutOfRangeException(nameof(dst.Rank), "The rank of the source and target arrays must match");
            }

            var dim = dst.Rank;
            var len = new int[dim];
            var idx = new int[dim];

            for(var i = 0; i < dim; ++i)
            {
                len[i] = Math.Min(src.GetLength(i), dst.GetLength(i));
            }

            while(idx[0] < len[0])
            {
                dst.SetValue(src.GetValue(idx), idx);

                for(var i = dim - 1; i >= 0; --i)
                {
                    if(++idx[i] < len[i])
                    {
                        break;
                    }
                }

                for(var i = 1; i < dim; ++i)
                {
                    idx[i] %= len[i];
                }
            }

            return dst;
        }

        public static bool IsEmpty<T>(this T[] items)
        {
            return items.Length <= 0;
        }

        public static bool IsNullOrEmpty<T>(this T[] items)
        {
            return !(items?.Length > 0);
        }

        /// <summary>
        ///     Resizes an array of any number of dimensions.
        /// </summary>
        public static Array Resize(this Array items, Type elementType, bool allowShrink, params int[] tar)
        {
            var dim = tar.Length;
            var len = new int[dim];
            var min = new int[dim];
            var max = new int[dim];

            for(var i = 0; i < dim; ++i)
            {
                if(tar[i] <= 0)
                {
                    throw new ArgumentOutOfRangeException();
                }

                len[i] = items?.GetLength(i) ?? 0;
                min[i] = Math.Min(len[i], tar[i]);
                max[i] = Math.Max(len[i], tar[i]);
            }

            tar = allowShrink ? min : max;

            return items.CopyTo(Array.CreateInstance(elementType, tar));
        }

        public static T[] Resize<T>(this T[] items, int count, bool allowShrink)
        {
            return (T[])Resize(items, typeof(T), allowShrink, count);
        }

        public static T[,] Resize<T>(this T[,] items, int countx, int county, bool allowShrink)
        {
            return (T[,])Resize(items, typeof(T), allowShrink, countx, county);
        }

        public static T[,,] Resize<T>(this T[,,] items, int countx, int county, int countz, bool allowShrink)
        {
            return (T[,,])Resize(items, typeof(T), allowShrink, countx, county, countz);
        }

        public static bool SetValue<T>(ref T[] items, int index, T value, IEqualityComparer<T> comparer = null)
        {
            if(index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if((items == null) || (index >= items.Length))
            {
                items = Resize(items, index + 1, false);
            }

            if((comparer != null) && comparer.Equals(items[index], value))
            {
                return false;
            }

            items[index] = value;
            return true;
        }

        /// <summary>
        ///     Treats the array as circular, allow indices outside the valid range to wrap around.
        /// </summary>
        public static bool SetWrappingValue<T>(this T[] items, int index, T value, IEqualityComparer<T> comparer = null)
        {
            if(items.IsNullOrEmpty())
            {
                throw new ArgumentOutOfRangeException(nameof(items), "Array may not be empty.");
            }

            if(index < 0)
            {
                index = -index;
                index %= items.Length;
                index = items.Length - index;
            }
            else
            {
                index %= items.Length;
            }

            return SetValue(items, index, value, comparer);
        }

        public static T[] ToArray<T>(this IEnumerable<T> items, Func<T, int> indexSelector, int minCount = 0)
        {
            var results = new T[minCount];

            foreach(var item in items)
            {
                var index = indexSelector(item);

                if(index < 0)
                {
                    continue;
                }

                SetValue(ref results, index, item);
            }

            return results;
        }
    }
}
