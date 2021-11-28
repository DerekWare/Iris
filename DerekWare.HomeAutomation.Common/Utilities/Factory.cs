using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using DerekWare.Collections;
using DerekWare.Diagnostics;

namespace DerekWare.HomeAutomation.Common
{
    // A factory is used to register and then create objects of the same base type. Reflection
    // A typical pattern would be to instantiate the object with the Factory, load any saved
    // properties from the PropertyCache, display the PropertyGrid, then save the changes back
    // to the PropertyCache.
    public interface IFactory<TObject, TProperties> : IReadOnlyObservableCollection<TProperties>
    {
        TObject CreateInstance(string name);
    }

    public interface IFactory<TObject> : IFactory<TObject, TObject>
    {
    }

    public interface ISerializableFactory<TObject, TProperties> : IFactory<TObject, TProperties>
    {
        void Deserialize(string cache);
        string Serialize();
    }

    public interface ISerializableFactory<TObject> : ISerializableFactory<TObject, TObject>
    {
    }

    public class Factory<TObject, TProperties> : ISerializableFactory<TObject, TProperties>
        where TProperties : ICloneable, IName where TObject : TProperties
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
            // Find all classes in all assemblies that subclass from TObject and add them
            // to the list automagically.
            AddRange(from type in Reflection.GetVisibleTypes()
                     where type.IsSubclassOf(typeof(TObject))
                     orderby type.GetName()
                     select type);
        }

        public int Count => Items.Count;
        public bool IsReadOnly => false;

        public void Add(Type type)
        {
            var obj = (TObject)Activator.CreateInstance(type);
            Items[obj.Name] = obj;
        }

        public void Add(TObject obj)
        {
            Items[obj.Name] = (TObject)obj.Clone();
        }

        public void AddRange(IEnumerable<Type> types)
        {
            types.ForEach(Add);
        }

        public void AddRange(IEnumerable<TObject> types)
        {
            types.ForEach(Add);
        }

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

        #region ISerializableFactory<TObject,TProperties>

        public void Deserialize(string cache)
        {
            try
            {
                // TODO validate the type exists in the app domain? There could be old entries.
                AddRange(JsonSerializer.Deserialize<List<TObject>>(cache));
            }
            catch(Exception ex)
            {
                Debug.Error(this, ex);
            }
        }

        public string Serialize()
        {
            return JsonSerializer.Serialize(Items.Values.ToList());
        }

        #endregion
    }
}
