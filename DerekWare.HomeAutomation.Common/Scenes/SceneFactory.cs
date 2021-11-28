using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using DerekWare.Collections;
using DerekWare.Diagnostics;

namespace DerekWare.HomeAutomation.Common.Scenes
{
    public class SceneFactory : IFactory<Scene, Scene>
    {
        public static readonly SceneFactory Instance = new();

        readonly SynchronizedHashSet<Scene> Items = new(EqualityComparer<Scene>.Default);

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => Items.CollectionChanged += value;
            remove => Items.CollectionChanged -= value;
        }

        public event PropertyChangedEventHandler PropertyChanged { add => Items.PropertyChanged += value; remove => Items.PropertyChanged -= value; }

        SceneFactory()
        {
        }

        public int Count => Items.Count;

        public void Deserialize(string text)
        {
            List<Scene> items;

            try
            {
                items = JsonSerializer.Deserialize<List<Scene>>(text);
            }
            catch(Exception ex)
            {
                Debug.Error(this, ex);
                return;
            }

            Items.AddRange(items.SafeEmpty());
        }

        public bool Remove(Scene scene)
        {
            return Items.Remove(scene);
        }

        public string Serialize()
        {
            return JsonSerializer.Serialize(Items.ToList());
        }

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEnumerable<Scene>

        public IEnumerator<Scene> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        #endregion

        #region IFactory<Scene,Scene>

        public Scene CreateInstance(string name)
        {
            var scene = new Scene(name);

            if(!Items.Add(scene))
            {
                throw new ArgumentException("A scene with that name already exists");
            }

            return scene;
        }

        #endregion
    }
}
