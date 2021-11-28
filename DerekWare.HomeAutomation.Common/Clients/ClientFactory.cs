using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using DerekWare.Collections;

namespace DerekWare.HomeAutomation.Common
{
    public class ClientFactory : IFactory<IClient, IClient>
    {
        public static readonly ClientFactory Instance = new();

        readonly SynchronizedList<IClient> Items = new();

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => Items.CollectionChanged += value;
            remove => Items.CollectionChanged -= value;
        }

        public event PropertyChangedEventHandler PropertyChanged { add => Items.PropertyChanged += value; remove => Items.PropertyChanged -= value; }

        ClientFactory()
        {
            Items.AddRange(from type in Reflection.GetVisibleTypes()
                           where type.GetInterfaces().Contains(typeof(IClient))
                           let instance = (IClient)type.GetField("Instance").GetValue(null)
                           select instance);
        }

        public int Count => Items.Count;

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Items).GetEnumerator();
        }

        #endregion

        #region IEnumerable<IClient>

        public IEnumerator<IClient> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        #endregion

        #region IFactory<IClient,IClient>

        public IClient CreateInstance(string family)
        {
            return Items.WhereEquals(i => i.Family, family).FirstOrDefault();
        }

        #endregion
    }
}
