using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DerekWare.Collections
{
    public enum SequenceComparisonType
    {
        /// <summary>
        ///     Compare the contents and order of the sequence.
        /// </summary>
        Sequence,

        /// <summary>
        ///     Compare just the contents of the sequence, ignoring order.
        /// </summary>
        Contents
    }

    public class SequenceComparer<T> : IEqualityComparer<IEnumerable<T>>, IEqualityComparer
    {
        public static readonly SequenceComparer<T> Default = new(SequenceComparisonType.Sequence);
        public static readonly SequenceComparer<T> DefaultIgnoreOrder = new(SequenceComparisonType.Contents);

        public readonly SequenceComparisonType ComparisonType;
        public readonly IEqualityComparer<T> EqualityComparer;

        public SequenceComparer(SequenceComparisonType comparisonType, IEqualityComparer<T> equalityComparer = null)
        {
            ComparisonType = comparisonType;
            EqualityComparer = equalityComparer ?? EqualityComparer<T>.Default;
        }

        #region Equality

        public new bool Equals(object left, object right)
        {
            return Equals(left as IEnumerable<T>, right as IEnumerable<T>);
        }

        public bool Equals(IEnumerable<T> left, IEnumerable<T> right)
        {
            if(ReferenceEquals(left, right))
            {
                return true;
            }

            if(ReferenceEquals(null, left) || ReferenceEquals(null, right))
            {
                return false;
            }

            switch(ComparisonType)
            {
                case SequenceComparisonType.Sequence:
                    return left.SequenceEqual(right, EqualityComparer);

                case SequenceComparisonType.Contents:
                    return new HashSet<T>(left, EqualityComparer).SetEquals(right);

                default:
                    throw new NotSupportedException("Invalid comparison type");
            }
        }

        public int GetHashCode(object obj)
        {
            if(obj is IEnumerable<T> collection)
            {
                return GetHashCode(collection);
            }

            return obj.GetHashCode();
        }

        public int GetHashCode(IEnumerable<T> collection)
        {
            var value = 0;

            foreach(var i in collection.SafeEmpty())
            {
                value ^= i?.GetHashCode() ?? 0;
            }

            return value;
        }

        #endregion

        /// <summary>
        ///     Determines for how many items the two collections are identical.
        /// </summary>
        /// <returns>
        ///     The index at which the two collections become different.
        /// </returns>
        public static int GetDivergence(IReadOnlyList<T> a, IReadOnlyList<T> b, IEqualityComparer<T> comparer = null)
        {
            comparer = comparer ?? EqualityComparer<T>.Default;

            var c = Math.Min(a.Count, b.Count);
            var i = 0;

            while((i < c) && comparer.Equals(a[i], b[i]))
            {
                ++i;
            }

            return i;
        }

        /// <summary>
        ///     Determines for how many items the two collections are identical.
        /// </summary>
        /// <returns>
        ///     The index at which the two collections become different.
        /// </returns>
        public static int GetDivergence(IEnumerable<T> a, IEnumerable<T> b, IEqualityComparer<T> comparer = null)
        {
            return GetDivergence(a.ToList(), b.ToList(), comparer);
        }
    }
}
