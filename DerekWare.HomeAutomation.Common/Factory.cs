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
            AddRange(from type in Reflection.GetVisibleTypes()
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
}
