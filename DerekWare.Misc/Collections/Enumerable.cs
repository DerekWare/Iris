using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DerekWare.Collections
{
    public class Indexed<T>
    {
        public readonly int Index;
        public readonly T Value;

        public Indexed(int index, T value)
        {
            Index = index;
            Value = value;
        }
    }

    public class IndexedEnumerator<T> : IEnumerator<T>
    {
        readonly int Count;
        readonly Func<int, T> Selector;

        T _Current;
        int Index = -1;

        public IndexedEnumerator(int count, Func<int, T> selector)
        {
            Count = count;
            Selector = selector;
        }

        public T Current => (Index >= 0) && (Index < Count) ? _Current : throw new IndexOutOfRangeException();
        object IEnumerator.Current => Current;

        #region IDisposable

        public void Dispose()
        {
        }

        #endregion

        #region IEnumerator

        public bool MoveNext()
        {
            if(++Index >= Count)
            {
                return false;
            }

            _Current = Selector(Index);
            return true;
        }

        public void Reset()
        {
            Index = -1;
        }

        #endregion
    }

    public static partial class Enumerable
    {
        public static IReadOnlyList<T> AsList<T>(this IEnumerable<T> items)
        {
            return items is IReadOnlyList<T> list ? list : new List<T>(items.SafeEmpty());
        }

        public static IReadOnlyCollection<T> AsCollection<T>(this IEnumerable<T> items)
        {
            return items is IReadOnlyCollection<T> collection ? collection : new List<T>(items.SafeEmpty());
        }

        #region Equality

        public static T FirstEquals<T>(this IEnumerable<T> @this, T value, IEqualityComparer<T> comparer = null)
        {
            return @this.WhereEquals(value, comparer).FirstOrDefault();
        }

        public static T1 FirstEquals<T1, T2>(this IEnumerable<T1> @this, Func<T1, T2> selector, T2 value, IEqualityComparer<T2> comparer = null)
        {
            return @this.WhereEquals(selector, value, comparer).FirstOrDefault();
        }

        public static T FirstEquals<T>(
            this IEnumerable<T> items,
            Func<T, string> selector,
            string value,
            StringComparison comparison = StringComparison.Ordinal)
        {
            return items.WhereEquals(selector, value, comparison).FirstOrDefault();
        }

        public static T FirstNotEquals<T>(this IEnumerable<T> @this, T value, IEqualityComparer<T> comparer = null)
        {
            return @this.WhereNotEquals(value, comparer).FirstOrDefault();
        }

        public static T1 FirstNotEquals<T1, T2>(this IEnumerable<T1> @this, Func<T1, T2> selector, T2 value, IEqualityComparer<T2> comparer = null)
        {
            return @this.WhereNotEquals(selector, value, comparer).FirstOrDefault();
        }

        public static T FirstNotEquals<T>(
            this IEnumerable<T> items,
            Func<T, string> selector,
            string value,
            StringComparison comparison = StringComparison.Ordinal)
        {
            return items.WhereNotEquals(selector, value, comparison).FirstOrDefault();
        }

        public static IEnumerable<T> WhereEquals<T>(this IEnumerable<T> @this, T value, IEqualityComparer<T> comparer = null)
        {
            comparer = comparer ?? EqualityComparer<T>.Default;
            return @this.Where(v => comparer.Equals(v, value));
        }

        public static IEnumerable<T1> WhereEquals<T1, T2>(this IEnumerable<T1> @this, Func<T1, T2> selector, T2 value, IEqualityComparer<T2> comparer = null)
        {
            comparer = comparer ?? EqualityComparer<T2>.Default;
            return @this.Where(v => comparer.Equals(selector(v), value));
        }

        public static IEnumerable<T> WhereEquals<T>(
            this IEnumerable<T> items,
            Func<T, string> selector,
            string value,
            StringComparison comparison = StringComparison.Ordinal)
        {
            return items.Where(v => string.Equals(selector(v), value, comparison));
        }

        public static IEnumerable<T> WhereNotEquals<T>(this IEnumerable<T> @this, T value, IEqualityComparer<T> comparer = null)
        {
            comparer = comparer ?? EqualityComparer<T>.Default;
            return @this.Where(v => !comparer.Equals(v, value));
        }

        public static IEnumerable<T1> WhereNotEquals<T1, T2>(this IEnumerable<T1> @this, Func<T1, T2> selector, T2 value, IEqualityComparer<T2> comparer = null)
        {
            comparer = comparer ?? EqualityComparer<T2>.Default;
            return @this.Where(v => !comparer.Equals(selector(v), value));
        }

        public static IEnumerable<T> WhereNotEquals<T>(
            this IEnumerable<T> items,
            Func<T, string> selector,
            string value,
            StringComparison comparison = StringComparison.Ordinal)
        {
            return items.Where(v => !string.Equals(selector(v), value, comparison));
        }

        #endregion

        #region Enumerable

        public static IEnumerable<T> AsEnumerable<T>(this T item)
        {
            return new[] { item };
        }

        public static IEnumerable<T> AsEnumerable<T>(params T[] items)
        {
            return items.SafeEmpty();
        }

        /// <summary>
        ///     Creates a copy of the collection before returning an enumerator so modifications to the source collection don't
        ///     affect an enumeration in flight.
        /// </summary>
        public static IEnumerator<T> CreateEnumerator<T>(this IEnumerable items)
        {
            return new List<T>(items.OfType<T>()).GetEnumerator();
        }

        /// <summary>
        ///     Creates a copy of the collection before returning an enumerator so modifications to the source collection don't
        ///     affect an enumeration in flight.
        /// </summary>
        public static IEnumerator<T> CreateEnumerator<T>(this IEnumerable<T> items)
        {
            return new List<T>(items).GetEnumerator();
        }

        /// <summary>
        ///     Creates an enumerator for an array-style collection.
        /// </summary>
        public static IEnumerator<T> CreateEnumerator<T>(int count, Func<int, T> selector)
        {
            return new IndexedEnumerator<T>(count, selector);
        }

        #endregion

#if false
        public static IEnumerable<T> Append<T>(this IEnumerable<T> @this, T item)
        {
            return @this.Concat(item.AsEnumerable());
        }
#endif

        public static IEnumerable<T> Append<T>(this IEnumerable<T> @this, params T[] items)
        {
            return @this.SafeEmpty().Concat(items.SafeEmpty());
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> @this, IEnumerable<T> items)
        {
            return @this.SafeEmpty().Concat(items.SafeEmpty());
        }

        /// <summary>
        ///     Returns a flattened list of all nodes in a tree. This is basically a recursive version of SelectMany that works
        ///     with a typed enumerable and maintains the original element ordering.
        /// </summary>
        public static DistinctList<T> Collapse<T>(
            this T root,
            Func<T, IEnumerable<T>> selector = null,
            IEqualityComparer<T> comparer = null,
            int depth = int.MaxValue)
        {
            var results = new DistinctList<T>(comparer);
            results.Collapse(root, selector, depth);
            return results;
        }

        /// <summary>
        ///     Returns a flattened list of all nodes in a tree. This is basically a recursive version of SelectMany that works
        ///     with a typed enumerable and maintains the original element ordering.
        /// </summary>
        public static DistinctList<T> Collapse<T>(
            this IEnumerable<T> collection,
            Func<T, IEnumerable<T>> selector = null,
            IEqualityComparer<T> comparer = null,
            int depth = int.MaxValue)
        {
            var results = new DistinctList<T>(comparer);
            collection.ForEach(item => results.Collapse(item, selector, depth));
            return results;
        }

        /// <summary>
        ///     Copies all elements of the collection to an array.
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="this"></param>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        /// <returns>The count of items copied.</returns>
        public static int CopyTo<TIn, TOut>(this IEnumerable<TIn> @this, TOut[] array, int arrayIndex)
            where TIn : TOut
        {
            return @this.ForEach<TIn>(i => array[arrayIndex++] = i);
        }

        /// <summary>
        ///     Provides the same unique selection functionality as System.Linq.Enumerable.Distinct, but maintains item order. This
        ///     method also performs lazy evaluation, where the default Distinct extension uses the UnionWith method, which
        ///     evaluates all items in the enumerable up-front.
        /// </summary>
        public static IEnumerable<T> Distinct<T>(IEnumerable<T> @this, IEqualityComparer<T> comparer = null)
        {
            var set = new HashSet<T>(comparer ?? EqualityComparer<T>.Default);
            return @this.Where(set.Add);
        }

        public static IEnumerable<T> Empty<T>()
        {
            return new T[0];
        }

        public static T FirstNotNull<T>(this IEnumerable<T> @this)
        {
            return @this.WhereNotNull().FirstOrDefault();
        }

        public static T1 FirstNotNull<T1, T2>(this IEnumerable<T1> @this, Func<T1, T2> selector)
        {
            return @this.WhereNotNull(selector).FirstOrDefault();
        }

        public static T FirstNotNull<T>(params T[] @this)
        {
            return FirstNotNull((IEnumerable<T>)@this);
        }

        public static T FirstNotNull<T>(this IEnumerable<T> items, Func<T, string> selector)
        {
            return items.WhereNotNull(selector).FirstOrDefault();
        }

        public static T FirstNull<T>(this IEnumerable<T> @this)
        {
            return @this.WhereNull().FirstOrDefault();
        }

        public static T1 FirstNull<T1, T2>(this IEnumerable<T1> @this, Func<T1, T2> selector)
        {
            return @this.WhereNull(selector).FirstOrDefault();
        }

        public static T FirstNull<T>(params T[] @this)
        {
            return FirstNull((IEnumerable<T>)@this);
        }

        public static T FirstNull<T>(this IEnumerable<T> items, Func<T, string> selector)
        {
            return items.WhereNull(selector).FirstOrDefault();
        }

        /// <summary>
        ///     Performs <paramref name="action" /> a number of times.
        /// </summary>
        public static void For(this int count, Action action)
        {
            for(var i = 0; i < count; ++i)
            {
                action();
            }
        }

        /// <summary>
        ///     Performs <paramref name="action" /> a number of times.
        /// </summary>
        public static void For(this int count, Action<int> action)
        {
            for(var i = 0; i < count; ++i)
            {
                action(i);
            }
        }

        /// <summary>
        ///     Performs <paramref name="action" /> a number of times.
        /// </summary>
        public static List<TResult> For<TResult>(this int count, Func<TResult> action)
        {
            var result = new List<TResult>();

            for(var i = 0; i < count; ++i)
            {
                result.Add(action());
            }

            return result;
        }

        /// <summary>
        ///     Performs <paramref name="action" /> a number of times.
        /// </summary>
        public static List<TResult> For<TResult>(this int count, Func<int, TResult> action)
        {
            var result = new List<TResult>();

            for(var i = 0; i < count; ++i)
            {
                result.Add(action(i));
            }

            return result;
        }

        /// <summary>
        ///     Performs <paramref name="action" /> on all items in the collection. See also ParallelEnumerable.ForAll.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="action"></param>
        public static int ForEach<T>(this IEnumerable<T> @this, Action<T> action)
        {
            var count = 0;

            foreach(var i in @this.SafeEmpty())
            {
                action(i);
                ++count;
            }

            return count;
        }

        /// <summary>
        ///     Performs <paramref name="action" /> on all items in the collection. See also ParallelEnumerable.ForAll.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="action"></param>
        public static List<TResult> ForEach<TItem, TResult>(this IEnumerable<TItem> @this, Func<TItem, TResult> action)
        {
            return @this.SafeEmpty().Select(action).ToList();
        }

        /// <summary>
        ///     Performs <paramref name="action" /> on all items in the collection that match the given type. See also
        ///     ParallelEnumerable.ForAll.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="action"></param>
        public static int ForEach<T>(this IEnumerable @this, Action<T> action)
        {
            var count = 0;

            foreach(var i in @this.SafeEmpty().OfType<T>())
            {
                action(i);
                ++count;
            }

            return count;
        }

        /// <summary>
        ///     Performs <paramref name="action" /> on all items in the collection. See also ParallelEnumerable.ForAll.
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="this"></param>
        /// <param name="action"></param>
        public static List<TResult> ForEach<TItem, TResult>(this IEnumerable @this, Func<TItem, TResult> action)
        {
            return @this.SafeEmpty().OfType<TItem>().Select(action).ToList();
        }

        /// <summary>
        ///     Performs <paramref name="action" /> on all items in the collection. See also ParallelEnumerable.ForAll.
        /// </summary>
        public static IEnumerable<TResult> ForEachMany<TItem, TResult>(this IEnumerable<TItem> @this, Func<TItem, IEnumerable<TResult>> action)
        {
            return @this.ForEach(action).SelectMany(i => i);
        }

        /// <summary>
        ///     Performs <paramref name="action" /> on all items in the collection. See also ParallelEnumerable.ForAll.
        /// </summary>
        public static IEnumerable<TResult> ForEachMany<TItem, TResult>(this IEnumerable @this, Func<TItem, IEnumerable<TResult>> action)
        {
            return @this.ForEach(action).SelectMany(i => i);
        }

        /// <summary>
        ///     Performs <paramref name="action" /> a number of times.
        /// </summary>
        public static IEnumerable<TResult> ForMany<TResult>(this int count, Func<int, IEnumerable<TResult>> action)
        {
            return count.For(action).SelectMany(i => i);
        }

        public static T GetNextValue<T>(this IList<T> @this, T value, int delta = 1)
        {
            var index = @this.IndexOf(value);

            if(index < 0)
            {
                return default;
            }

            index += delta;
            index %= @this.Count;

            while(index < 0)
            {
                index += @this.Count;
            }

            return @this[index];
        }

        public static TValue GetValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> @this, TKey key)
        {
            @this.TryGetValue(key, out var value);
            return value;
        }

        public static TValue GetValue<TKey, TValue>(this IReadOnlyLookup<TKey, TValue> @this, TKey key)
        {
            @this.TryGetValue(key, out var value);
            return value;
        }

        public static TValue GetValueOrNew<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, params object[] args)
        {
            return GetValueOrNew<TKey, TValue, TValue>(@this, key, args);
        }

        public static TNew GetValueOrNew<TKey, TValue, TNew>(this IDictionary<TKey, TValue> @this, TKey key, params object[] args)
            where TNew : TValue
        {
            if(!@this.TryGetValue(key, out var value) || (null == value))
            {
                @this[key] = value = (TValue)Activator.CreateInstance(typeof(TNew), args.IsNullOrEmpty() ? null : args);
            }

            return (TNew)value;
        }

        public static TValue GetValueOrNew<TKey, TValue>(this ILookup<TKey, TValue> @this, TKey key, params object[] args)
        {
            return GetValueOrNew<TKey, TValue, TValue>(@this, key, args);
        }

        public static TNew GetValueOrNew<TKey, TValue, TNew>(this ILookup<TKey, TValue> @this, TKey key, params object[] args)
            where TNew : TValue
        {
            if(!@this.TryGetValue(key, out var value) || (null == value))
            {
                @this[key] = value = (TValue)Activator.CreateInstance(typeof(TNew), args.IsNullOrEmpty() ? null : args);
            }

            return (TNew)value;
        }

        public static IEnumerable<Indexed<T>> Index<T>(this IEnumerable<T> @this, int start = 0)
        {
            return @this.Select(item => new Indexed<T>(start++, item));
        }

        public static List<Indexed<T>> Index<T>(this IEnumerable<T> @this, Func<T, int> indexSelector)
        {
            var results = new List<Indexed<T>>();

            foreach(var item in @this)
            {
                var index = indexSelector(item);

                if(index < 0)
                {
                    continue;
                }

                while(index >= results.Count)
                {
                    results.Add(new Indexed<T>(results.Count, default));
                }

                results[index] = new Indexed<T>(index, item);
            }

            return results;
        }

        public static int IndexOf<T1, T2>(this IEnumerable<T1> items, T2 value)
        {
            var i = 0;

            foreach(var item in items)
            {
                if(Equals(item, value))
                {
                    return i;
                }

                ++i;
            }

            return -1;
        }

        public static bool IsNullOrEmpty<T>(this LinkedList<T> @this)
        {
            return @this?.First is null;
        }

        public static bool IsNullOrEmpty<T>(this ICollection<T> @this)
        {
            return !(@this?.Count > 0);
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> @this)
        {
            return @this?.Any() != true;
        }

        public static bool Overlaps<T>(this IEnumerable<T> left, IEnumerable<T> right, IEqualityComparer<T> comparer = null)
        {
            return new HashSet<T>(left, comparer ?? EqualityComparer<T>.Default).Overlaps(right);
        }

#if false
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> @this, T item)
        {
            return item.AsEnumerable().Concat(@this.SafeEmpty());
        }
#endif

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> @this, params T[] items)
        {
            return items.SafeEmpty().Concat(@this.SafeEmpty());
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> @this, IEnumerable<T> items)
        {
            return items.SafeEmpty().Concat(@this.SafeEmpty());
        }

        public static IEnumerable<int> Range(this int count)
        {
            return Range(0, 1, count);
        }

        public static IEnumerable<int> Range(int start, int count)
        {
            return Range(start, 1, count);
        }

        public static IEnumerable<int> Range(int start, int step, int count)
        {
            for(var i = start; i < count; i += step)
            {
                yield return i;
            }
        }

        public static void RemoveAfter<T>(this LinkedList<T> @this, LinkedListNode<T> node)
        {
            node = node?.Next;

            while(null != node)
            {
                var next = node.Next;
                @this.Remove(node);
                node = next;
            }
        }

        public static void RemoveBefore<T>(this LinkedList<T> @this, LinkedListNode<T> node)
        {
            node = node?.Previous;

            while(null != node)
            {
                var next = node.Previous;
                @this.Remove(node);
                node = next;
            }
        }

        public static void RemoveBetween<T>(this LinkedList<T> @this, LinkedListNode<T> first, LinkedListNode<T> last)
        {
            var node = first?.Next;

            while((null != node) && (last != node))
            {
                var next = node.Next;
                @this.Remove(node);
                node = next;
            }
        }

        public static int RemoveWhere<T>(this ICollection<T> @this, Func<T, bool> selector)
        {
            return @this.SafeEmpty().Where(selector).ToList().Count(@this.Remove);
        }

        public static int RemoveWhere<T>(this IList<T> @this, Func<T, bool> selector)
        {
            return @this.SafeEmpty().Where(selector).ToList().Count(@this.Remove);
        }

        public static void RemoveWhere<T>(this IList @this, Func<T, bool> selector)
        {
            @this.OfType<T>().Where(selector).ToList().ForEach(item => @this.Remove(item));
        }

        public static int RemoveWhere<T>(this LinkedList<T> @this, Func<T, bool> selector)
        {
            var c = @this?.First;
            var i = 0;

            while(null != c)
            {
                var n = c.Next;

                if(selector(c.Value))
                {
                    @this.Remove(c);
                    ++i;
                }

                c = n;
            }

            return i;
        }

        public static IEnumerable<T> Repeat<T>(this IEnumerable<T> @this, int count)
        {
            foreach(var item in @this)
            {
                for(var i = 0; i < count; ++i)
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<T> Repeat<T>(this IEnumerable<T> @this, Func<T, int> getCount)
        {
            foreach(var item in @this)
            {
                var count = getCount(item);

                for(var i = 0; i < count; ++i)
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<T> Repeat<T>(this T item, int count)
        {
            for(var i = 0; i < count; ++i)
            {
                yield return item;
            }
        }

        public static IEnumerable<T> Repeat<T>(this T item, Func<T, int> getCount)
        {
            return Repeat(item, getCount(item));
        }

        public static IEnumerable<T> SafeEmpty<T>(this IEnumerable<T> @this)
        {
            return @this ?? Empty<T>();
        }

        public static IEnumerable SafeEmpty(this IEnumerable @this)
        {
            return @this ?? Empty<object>();
        }

        public static bool SetValue<T>(this IList<T> @this, int index, T value, IEqualityComparer<T> comparer = null)
        {
            if(index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            ((index + 1) - @this.Count).For(i => @this.Add(default));

            if((null != comparer) && comparer.Equals(@this[index], value))
            {
                return false;
            }

            @this[index] = value;
            return true;
        }

        public static bool SetValue<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, TValue value, IEqualityComparer<TValue> comparer = null)
        {
            if((null != comparer) && @this.TryGetValue(key, out var c) && comparer.Equals(c, value))
            {
                return false;
            }

            @this[key] = value;
            return true;
        }

        public static bool SetValue<TKey, TValue>(this ILookup<TKey, TValue> @this, TKey key, TValue value, IEqualityComparer<TValue> comparer = null)
        {
            if((null != comparer) && @this.TryGetValue(key, out var c) && comparer.Equals(c, value))
            {
                return false;
            }

            @this[key] = value;
            return true;
        }

        public static List<T> Shuffle<T>(this IEnumerable<T> @this, Random random = null)
        {
            random = random ?? new Random();
            var result = new List<T>();
            @this.ForEach(i => result.Insert(random.Next(result.Count + 1), i));
            return result;
        }

        public static List<T> Sort<T>(this IEnumerable<T> @this, IComparer<T> comparer = null)
        {
            comparer = comparer ?? Comparer<T>.Default;
            var result = @this.ToList();
            result.Sort(comparer);
            return result;
        }

        public static T[] ToArray<T>(this IEnumerable @this)
        {
            return @this.Cast<T>().ToArray();
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> @this,
            IEqualityComparer<TKey> comparer = null)
        {
            var dictionary = new Dictionary<TKey, TValue>(comparer ?? EqualityComparer<TKey>.Default);
            @this.ForEach(i => dictionary.Add(i.Key, i.Value));
            return dictionary;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
            this IEnumerable<TValue> @this,
            Func<TValue, TKey> keySelector,
            IEqualityComparer<TKey> comparer = null)
        {
            var dictionary = new Dictionary<TKey, TValue>(comparer ?? EqualityComparer<TKey>.Default);
            @this.ForEach(i => dictionary.Add(keySelector(i), i));
            return dictionary;
        }

        /// <summary>
        ///     This is functionally equivalent to Distinct().ToList(), except that it returns a collection object that extends the
        ///     functionality of List.
        /// </summary>
        public static DistinctList<T> ToDistinctList<T>(this IEnumerable<T> @this, IEqualityComparer<T> comparer = null)
        {
            return new DistinctList<T>(@this, comparer);
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> @this, IEqualityComparer<T> comparer = null)
        {
            return new HashSet<T>(@this, comparer ?? EqualityComparer<T>.Default);
        }

        public static LinkedList<T> ToLinkedList<T>(this IEnumerable<T> @this)
        {
            return new LinkedList<T>(@this);
        }

        public static Queue<T> ToQueue<T>(this IEnumerable<T> @this)
        {
            return new Queue<T>(@this);
        }

        public static Stack<T> ToStack<T>(this IEnumerable<T> @this)
        {
            return new Stack<T>(@this);
        }

        public static IEnumerable<T> WhereNot<T>(this IEnumerable<T> items, Func<T, bool> selector)
        {
            return items.Where(v => !selector(v));
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> items, Func<T, string> selector)
        {
            return items.Where(v => !string.IsNullOrEmpty(selector(v)));
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> @this)
        {
            return @this.Where(v => null != v);
        }

        public static IEnumerable<T1> WhereNotNull<T1, T2>(this IEnumerable<T1> @this, Func<T1, T2> selector)
        {
            return @this.Where(v => null != selector(v));
        }

        public static IEnumerable<T> WhereNull<T>(this IEnumerable<T> items, Func<T, string> selector)
        {
            return items.Where(v => string.IsNullOrEmpty(selector(v)));
        }

        public static IEnumerable<T> WhereNull<T>(this IEnumerable<T> @this)
        {
            return @this.Where(v => null == v);
        }

        public static IEnumerable<T1> WhereNull<T1, T2>(this IEnumerable<T1> @this, Func<T1, T2> selector)
        {
            return @this.Where(v => null == selector(v));
        }

        /// <summary>
        ///     Returns a flattened list of all nodes in a tree. This is basically a recursive version of SelectMany that works
        ///     with a typed enumerable and maintains the original element ordering.
        /// </summary>
        static void Collapse<T>(this DistinctList<T> results, T root, Func<T, IEnumerable<T>> selector, int depth)
        {
            selector = selector ?? (item => item as IEnumerable<T>);

            // Add the root object to prevent recursion where children point back to a parent
            var offset = results.Count;
            results.Add(root);
            var length = results.Count;

            // Start appending child elements to the result list in blocks bounded by offset and length, 
            // only updating length after all child elements at this level have been added. This allows 
            // us to recursively scan through children without method recursion and without modifying 
            // an active enumerable.
            while((--depth >= 0) && (offset < length))
            {
                var children = 0;

                while(offset < length)
                {
                    children += results.AddRange(selector(results[offset++]));
                }

                length += children;
            }
        }
    }
}
