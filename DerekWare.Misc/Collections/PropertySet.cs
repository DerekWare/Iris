using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using DerekWare.Diagnostics;
using DerekWare.Reflection;
using DerekWare.Strings;

namespace DerekWare.Collections
{
    /// <summary>
    ///     The optional source of data for the property set.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IPropertySetSource<TKey>
    {
        /// <summary>
        ///     Returns true if the owning PropertySet should call Read instead of GetValue because it's more optimal.
        /// </summary>
        bool AutoLoad { get; }

        /// <summary>
        ///     Retrieves a single value.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetValue(TKey key);

        /// <summary>
        ///     Retrieves all values.
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValuePair<TKey, string>> Load();

        /// <summary>
        ///     Stores all values.
        /// </summary>
        /// <param name="values"></param>
        void Save(IEnumerable<KeyValuePair<TKey, string>> values);
    }

    /// <summary>
    ///     A property set is a simplified dictionary of key/value pairs where the values are always stored as strings and
    ///     converted to and from specific types using the FormatString/Parse extension methods. Values returned are always a
    ///     valid string; never null. As an alternative, see System.Dynamic.ExpandoObject.
    /// </summary>
    public class PropertySet<TKey> : ObservableDictionary<TKey, string>
    {
        readonly HashSet<TKey> _ModifiedKeys;
        bool _AutoCommit = true;
        bool _AutoLoad;
        IPropertySetSource<TKey> _Source;

        public event PropertyChangedEventHandler PropertyCommitted;

        public PropertySet()
            : this(EqualityComparer<TKey>.Default)
        {
        }

        public PropertySet(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
            _ModifiedKeys = new HashSet<TKey>(comparer);
        }

        public PropertySet(IPropertySetSource<TKey> source)
            : this()
        {
            Source = source;
        }

        public PropertySet(IEnumerable source)
            : this()
        {
            Load(source);
        }

        public bool IsDirty => _ModifiedKeys.Count > 0;
        public IReadOnlyDictionary<TKey, string> Modified => _ModifiedKeys.Select(key => key.ToKeyValuePair(this[key])).ToDictionary();
        public int ModifiedCount => _ModifiedKeys.Count;
        public ICollection<TKey> ModifiedKeys => _ModifiedKeys;
        public ICollection<string> ModifiedValues => _ModifiedKeys.Select(GetValue).ToList();

        public bool AutoCommit
        {
            get => _AutoCommit;
            set
            {
                if(value && !_AutoCommit)
                {
                    Commit();
                }

                _AutoCommit = value;
            }
        }

        public bool AutoLoad
        {
            get => _AutoLoad;
            set
            {
                if((null != _Source) && _Source.AutoLoad)
                {
                    value = true;
                }

                if(value && !_AutoLoad)
                {
                    Load();
                }

                _AutoLoad = value;
            }
        }

        public IPropertySetSource<TKey> Source
        {
            get => _Source;
            set
            {
                if(value == _Source)
                {
                    return;
                }

                _Source = value;

                if((null != _Source) && _Source.AutoLoad)
                {
                    _AutoLoad = true;
                }

                if(_AutoLoad)
                {
                    Load();
                }
            }
        }

        public override void Add(TKey key, string value)
        {
            SetValue(key, value);
        }

        public override void Clear()
        {
            _ModifiedKeys.Clear();
            base.Clear();
        }

        public void Commit()
        {
            if(!IsDirty)
            {
                return;
            }

            _Source?.Save(this);

            if(null != PropertyCommitted)
            {
                _ModifiedKeys.ForEach(key => PropertyCommitted.Invoke(this, new PropertyChangedEventArgs(key.ToString())));
            }

            _ModifiedKeys.Clear();
        }

        public override string GetValue(TKey key)
        {
            TryGetValue(key, out var value);
            return value;
        }

        public T GetValue<T>(TKey key)
        {
            TryGetValue<T>(key, out var value);
            return value;
        }

        public void Load(bool force = false)
        {
            Load(_Source?.Load(), force);
        }

        public void Load(IEnumerable<KeyValuePair<TKey, string>> source, bool force = false)
        {
            _ModifiedKeys.Clear();

            if((null == source) || (!force && !IsDirty))
            {
                return;
            }

            foreach(var i in source)
            {
                SetValue(i.Key, i.Value);
            }
        }

        public void Load(IEnumerable source, bool force = false)
        {
            _ModifiedKeys.Clear();

            if((null == source) || (!force && !IsDirty))
            {
                return;
            }

            foreach(var i in source)
            {
                if(TryParse(i, out var j))
                {
                    SetValue(j.Key, j.Value);
                }
            }
        }

        public override bool Remove(TKey key)
        {
            return base.Remove(key) || _ModifiedKeys.Remove(key);
        }

        public override bool SetValue(TKey key, string value)
        {
            if(!base.SetValue(key, value ?? ""))
            {
                return false;
            }

            _ModifiedKeys.Add(key);

            if(AutoCommit)
            {
                Commit();
            }

            return true;
        }

        public bool SetValue(TKey key, object value)
        {
            if(!base.SetValue(key, value.SafeToString()))
            {
                return false;
            }

            _ModifiedKeys.Add(key);

            if(AutoCommit)
            {
                Commit();
            }

            return true;
        }

        public bool SetValue(KeyValuePair<TKey, string> value)
        {
            return SetValue(value.Key, value.Value);
        }

        public bool SetValue<TValue>(KeyValuePair<TKey, TValue> value)
        {
            return SetValue(value.Key, value.Value.SafeToString());
        }

        public int SetValues(IEnumerable<KeyValuePair<TKey, string>> values)
        {
            return values.Count(v => SetValue(v));
        }

        public int SetValues<TValue>(IEnumerable<KeyValuePair<TKey, TValue>> values)
        {
            return values.Count(v => SetValue(v));
        }

        public TCollection ToCollection<TCollection, TTargetKey, TTargetValue>()
            where TCollection : ICollection<KeyValuePair<TTargetKey, TTargetValue>>, new()
        {
            var result = new TCollection();

            foreach(var i in this)
            {
                var key = i.Key.Convert<TTargetKey>();
                var value = i.Value.Convert<TTargetValue>();

                result.Add(new KeyValuePair<TTargetKey, TTargetValue>(key, value));
            }

            return result;
        }

        public Dictionary<TKey, string> ToDictionary(IEqualityComparer<TKey> comparer = null)
        {
            return new Dictionary<TKey, string>(this, comparer ?? Comparer);
        }

        public Dictionary<TTargetKey, TTargetValue> ToDictionary<TTargetKey, TTargetValue>(IEqualityComparer<TTargetKey> comparer = null)
        {
            var result = new Dictionary<TTargetKey, TTargetValue>(comparer ?? EqualityComparer<TTargetKey>.Default);

            foreach(var i in this)
            {
                var key = i.Key.Convert<TTargetKey>();
                var value = i.Value.Convert<TTargetValue>();

                result.Add(key, value);
            }

            return result;
        }

        public ExpandoObject ToExpandoObject()
        {
            return ToCollection<ExpandoObject, string, object>();
        }

        public bool TryGetValue<T>(TKey key, out T value)
        {
            if(TryGetValue(key, out var s))
            {
                return s.TryConvert(out value);
            }

            value = default;
            return false;
        }

        public override bool TryGetValue(TKey key, out string value)
        {
            // Check the currently known values
            if(base.TryGetValue(key, out value))
            {
                return true;
            }

            // Load from the source. No matter what we get, cache the result so we don't call _Source.GetValue again.
            if((null != _Source) && !_Source.AutoLoad)
            {
                try
                {
                    value = _Source.GetValue(key);
                }
                catch(Exception ex)
                {
                    Debug.Trace(this, ex);
                }
            }

            value = value ?? "";

            base.SetValue(key, value);

            return !string.IsNullOrEmpty(value);
        }

        #region Conversion

        protected static KeyValuePair<TKey, string> Parse(object source)
        {
            var reflector = new Reflector(source);

            var _key = reflector.GetMember("Key").Evaluate();
            var _value = reflector.GetMember("Value").Evaluate();

            if(!(_key is TKey key))
            {
                _key.SafeToString().TryParse(out key);
            }

            if(!(_value is string value))
            {
                value = _value.SafeToString();
            }

            if(value.IsNullOrEmpty())
            {
                throw new ArgumentNullException("source.Value");
            }

            return new KeyValuePair<TKey, string>(key, value);
        }

        #endregion

        protected static bool TryParse(object source, out KeyValuePair<TKey, string> target)
        {
            try
            {
                target = Parse(source);
                return true;
            }
            catch
            {
                target = default;
                return false;
            }
        }
    }

    public class PropertySet : PropertySet<string>
    {
        public PropertySet()
        {
        }

        public PropertySet(IEqualityComparer<string> comparer)
            : base(comparer)
        {
        }

        public PropertySet(IPropertySetSource<string> source)
            : base(source)
        {
        }

        public PropertySet(IEnumerable source)
            : base(source)
        {
        }
    }
}
