using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using DerekWare.Collections;

namespace DerekWare.HomeAutomation.Common.Scenes
{
    /// <summary>
    ///     A Scene is a collection of colors, themes and effects applied to any number
    ///     of devices that can be persisted in the settings.
    /// </summary>
    public class Scene
    {
        public Scene()
        {
            Items.CollectionChanged += OnCollectionChanged;
        }

        public Scene(string name)
            : this()
        {
            Name = name;
        }

        public bool AutoApply { get; set; }

        public SynchronizedHashSet<SceneItem> Items { get; set; } = new();

        public string Name { get; set; }

        public bool Add(IDevice device)
        {
            return Items.Add(new SceneItem(device));
        }

        public int AddRange(IEnumerable<IDevice> devices)
        {
            return devices.ForEach(Add).Count();
        }

        public void Apply()
        {
            Items.ForEach(i => i.ApplyScene());
        }

        public bool Contains(IDevice device)
        {
            return Items.Any(i => i.Matches(device));
        }

        public bool Remove(IDevice device)
        {
            return Items.RemoveWhere(i => i.Matches(device)) > 0;
        }

        public int RemoveRange(IEnumerable<IDevice> devices)
        {
            return devices.ForEach(Remove).Count();
        }

        #region Event Handlers

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if((e.Action != NotifyCollectionChangedAction.Remove) && (e.Action != NotifyCollectionChangedAction.Reset))
            {
                return;
            }

            foreach(SceneItem item in e.OldItems)
            {
                item.Dispose();
            }
        }

        #endregion
    }
}
