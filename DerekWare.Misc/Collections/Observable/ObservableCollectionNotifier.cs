using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace DerekWare.Collections
{
    public interface IObservableCollectionNotifier : INotifyCollectionChanged, INotifyPropertyChanged
    {
    }

    public class ObservableCollectionNotifier<T> : IObservableCollectionNotifier
    {
        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;
        public virtual event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnAdd(T item)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public virtual void OnAdd(IList<T> items)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items));
        }

        public void OnAdd(IEnumerable<T> items)
        {
            OnAdd(items.SafeEmpty().ToList());
        }

        public virtual void OnMove(T item, int oldIndex, int newIndex)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex));
        }

        public virtual void OnRemove(T item)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        }

        public virtual void OnRemove(IList<T> items)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items.SafeEmpty().ToList()));
        }

        public void OnRemove(IEnumerable<T> items)
        {
            OnRemove(items.SafeEmpty().ToList());
        }

        public virtual void OnReplace(T oldItem, T newItem)
        {
            OnReplace(new[] { oldItem }, new[] { newItem });
        }

        public virtual void OnReplace(IList<T> oldItems, IList<T> newItems)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                                                                     oldItems.SafeEmpty().ToList(),
                                                                     newItems.SafeEmpty().ToList()));
        }

        public void OnReplace(IEnumerable<T> oldItems, IEnumerable<T> newItems)
        {
            OnReplace(oldItems.SafeEmpty().ToList(), newItems.SafeEmpty().ToList());
        }

        public virtual void OnReset(IList<T> items)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void OnReset(IEnumerable<T> items)
        {
            OnReset(items.SafeEmpty().ToList());
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);

            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Reset:
                    OnPropertyChanged("Count");
                    break;

                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
