using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace DerekWare.Collections
{
    public class ObservableDictionaryNotifier<TKey> : IObservableCollectionNotifier
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnAdd(TKey item)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(item.ToString()));
        }

        public virtual void OnAdd(IList<TKey> items)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));

            if(null != PropertyChanged)
            {
                items.ForEach(i => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(i.ToString())));
            }
        }

        public virtual void OnRemove(TKey item)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(item.ToString()));
        }

        public virtual void OnRemove(IList<TKey> items)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));

            if(null != PropertyChanged)
            {
                items.ForEach(i => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(i.ToString())));
            }
        }

        public virtual void OnReplace(TKey oldItem, TKey newItem)
        {
            OnReplace(new[] { oldItem }, new[] { newItem });
        }

        public virtual void OnReplace(IList<TKey> oldItems, IList<TKey> newItems)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldItems, newItems));

            if(null != PropertyChanged)
            {
                oldItems.ForEach(i => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(i.ToString())));
            }
        }

        public virtual void OnReset()
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
        }
    }
}
