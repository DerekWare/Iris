using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DerekWare.Collections;
using DerekWare.Diagnostics;
using DerekWare.Reflection;

namespace DerekWare.HomeAutomation.Common
{
    // A factory is used to register and then create objects of the same base type. Reflection
    // A typical pattern would be to instantiate the object with the Factory, load any saved
    // properties from the PropertyCache, display the PropertyGrid, then save the changes back
    // to the PropertyCache.
    public interface IFactory<out TObject> : IReadOnlyObservableCollection<TObject>
        where TObject : ICloneable, IName
    {
        TObject CreateInstance(string name);
    }

    public class Factory<TObject> : IFactory<TObject>
        where TObject : ICloneable, IName
    {
        protected readonly SynchronizedDictionary<string, TObject> Items = new();

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => Items.CollectionChanged += value;
            remove => Items.CollectionChanged -= value;
        }

        public event PropertyChangedEventHandler PropertyChanged { add => Items.PropertyChanged += value; remove => Items.PropertyChanged -= value; }

        public Factory()
        {
            // Find all classes in this assembly that subclass from TType and add them
            // to the list automagically.
            AddRange(from type in Factory.GetVisibleTypes()
                     where type.IsSubclassOf(typeof(TObject)) || type.GetInterfaces().Contains(typeof(TObject))
                     orderby type.GetName()
                     select type);
        }

        public int Count => Items.Count;
        public bool IsReadOnly => false;

        public void Add(Type type)
        {
            Add(() => (TObject)Activator.CreateInstance(type));
        }

        public void Add(Func<TObject> activator)
        {
            Add(activator());
        }

        public void Add(TObject obj)
        {
            Items.Add(obj.Name, obj);
        }

        public void AddRange(IEnumerable<Type> types)
        {
            types.ForEach(Add);
        }

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEnumerable<TObject>

        public IEnumerator<TObject> GetEnumerator()
        {
            return Items.Values.GetEnumerator();
        }

        #endregion

        #region IFactory<TObject>

        public TObject CreateInstance(string name)
        {
            return (TObject)Items[name].Clone();
        }

        #endregion
    }

    public static class Factory
    {
        public static string GetDescription(this Type type)
        {
            return type.GetCustomAttributes<DescriptionAttribute>(false).FirstOrDefault()?.Description;
        }

        public static string GetDescription(this object @this)
        {
            return GetDescription(@this.GetType());
        }

        public static string GetName(this Type type)
        {
            return type.GetCustomAttributes<NameAttribute>(false).FirstOrDefault()?.Name ?? type.Name;
        }

        public static string GetName(this object @this)
        {
            return GetName(@this.GetType());
        }

        public static IEnumerable<PropertyInfo> GetProperties(this Type type)
        {
            return from propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                   where propertyInfo.CanRead
                   let browsableAttributes = propertyInfo.GetCustomAttributes<BrowsableAttribute>(true).ToList()
                   where (browsableAttributes.Count <= 0) || browsableAttributes.First().Browsable
                   select propertyInfo;
        }

        public static IEnumerable<Type> GetVisibleTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(i => i.GetVisibleTypes());
        }

        public static IEnumerable<Type> GetVisibleTypes(this Assembly assembly)
        {
            return from type in assembly.GetTypes()
                   where !type.IsAbstract && type.IsVisible
                   let browsableAttributes = type.GetCustomAttributes<BrowsableAttribute>(true).ToList()
                   where (browsableAttributes.Count <= 0) || browsableAttributes.First().Browsable
                   select type;
        }

        public static IEnumerable<PropertyInfo> GetWritableProperties(Type type)
        {
            return from p in GetProperties(type)
                   where p.CanWrite
                   select p;
        }

        public static IReadOnlyDictionary<string, object> GetWritablePropertyValues(this object @this, Type type = null)
        {
            type ??= @this.GetType();

            return (from p in GetWritableProperties(type)
                    select new KeyValuePair<string, object>(p.Name, p.GetValue(@this))).ToDictionary();
        }

        public static bool SetPropertyValue(this object @this, string name, object value, Type type = null)
        {
            type ??= @this.GetType();

            var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);

            if(property is null || !property.CanWrite)
            {
                return false;
            }

            try
            {
                property.SetValue(@this, value);
            }
            catch(Exception e)
            {
                Debug.Warning(@this, e);
                return false;
            }

            return true;
        }

        public static int SetPropertyValues(this object @this, IReadOnlyDictionary<string, object> src, Type type = null)
        {
            return src.Sum(i => SetPropertyValue(@this, i.Key, i.Value, type) ? 1 : 0);
        }
    }
}
