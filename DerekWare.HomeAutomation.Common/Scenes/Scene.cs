using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.HomeAutomation.Common.Themes;

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
        }

        public Scene(string name)
        {
            Name = name;
        }

        public SynchronizedHashSet<SceneItem> Items { get; set; } = new();

        public string Name { get; set; }

        public bool Add(IDevice device)
        {
            return Items.Add(new SceneItem(device));
        }

        public void Apply()
        {
            Items.ForEach(i => i.Apply());
        }

        public bool Contains(IDevice device)
        {
            return Items.Any(i => i.Matches(device));
        }

        public bool Remove(IDevice device)
        {
            return Items.RemoveWhere(i => i.Matches(device)) > 0;
        }
    }
}
