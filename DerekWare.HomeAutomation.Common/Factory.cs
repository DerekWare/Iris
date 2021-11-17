using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using DerekWare.Collections;

namespace DerekWare.HomeAutomation.Common
{
    // A factory is used to register and then create objects of the same base type. Reflection
    // A typical pattern would be to instantiate the object with the Factory, load any saved
    // properties from the PropertyCache, display the PropertyGrid, then save the changes back
    // to the PropertyCache.
    public interface IFactory<out TObject, TProperties> : IObservableCollection<TProperties>
        where TObject : TProperties, ICloneable
    {
        TObject CreateInstance(string name);
    }

    public class Factory<TObject, TProperties> : IFactory<TObject, TProperties>
        where TObject : TProperties, ICloneable
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
            AddRange(from type in Reflection.GetVisibleTypes()
                     where type.IsSubclassOf(typeof(TObject)) || type.GetInterfaces().Contains(typeof(TObject))
                     orderby type.GetTypeName()
                     select type);
        }

        public int Count => Items.Count;
        public bool IsReadOnly => false;

        public void Add(Type type)
        {
            Add(type.GetTypeName(), () => (TObject)Activator.CreateInstance(type));
        }

        public void Add(string name, Func<TObject> activator)
        {
            Items.Add(name, activator());
        }

        public void AddRange(IEnumerable<Type> types)
        {
            types.ForEach(Add);
        }

        public bool Contains(string name)
        {
            return Items.ContainsKey(name);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            Items.Keys.CopyTo(array, arrayIndex);
        }

        public bool Remove(string name)
        {
            return Items.Remove(name);
        }

        #region ICollection<TProperties>

        public void Add(TProperties item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            Items.Clear();
        }

        public bool Contains(TProperties item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(TProperties[] array, int arrayIndex)
        {
            Items.Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(TProperties item)
        {
            return Items.RemoveWhere(i => Equals(i.Value, item)) > 0;
        }

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEnumerable<TProperties>

        public IEnumerator<TProperties> GetEnumerator()
        {
            return Items.Values.Cast<TProperties>().GetEnumerator();
        }

        #endregion

        #region IFactory<TObject,TProperties>

        public TObject CreateInstance(string name)
        {
            return (TObject)Items[name].Clone();
        }

        #endregion
    }
}
