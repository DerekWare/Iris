using System;
using System.Collections.Generic;
using System.Linq;
using DerekWare.Collections;

namespace DerekWare.HomeAutomation.Common.Scenes
{
    public interface IReadOnlySceneProperties : IName
    {
    }

    public interface ISceneProperties : IReadOnlySceneProperties
    {
        ICollection<SceneItem> Items { get; }
        new string Name { get; set; }
    }

    /// <summary>
    ///     A Scene is a collection of colors, themes and effects applied to any number
    ///     of devices that can be persisted in the settings.
    /// </summary>
    public class Scene : ISceneProperties, IEquatable<Scene>
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

        ICollection<SceneItem> ISceneProperties.Items => Items;

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
            return Items.Any(i => i.Equals(device));
        }

        public bool Remove(IDevice device)
        {
            return Items.RemoveWhere(i => i.Equals(device)) > 0;
        }

        #region Equality

        public bool Equals(Scene other)
        {
            if(ReferenceEquals(null, other))
            {
                return false;
            }

            if(ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj))
            {
                return false;
            }

            if(obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Scene)obj);
        }

        public override int GetHashCode()
        {
            return Name.IsNullOrEmpty() ? 0 : Name.GetHashCode();
        }

        public static bool operator ==(Scene left, Scene right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Scene left, Scene right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}
