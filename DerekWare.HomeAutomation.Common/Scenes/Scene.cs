using System.Collections.Generic;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.HomeAutomation.Common.Themes;

namespace DerekWare.HomeAutomation.Common.Scenes
{
    /// <summary>
    ///     A Scene is a collection of colors, themes and effects applied to any number of devices that can be persisted in
    ///     storage.
    /// </summary>
    public class Scene : List<SceneItem>, IName
    {
        public string Name { get; set; }
    }

    public class SceneItem
    {
        public SceneItem()
        {
        }

        public SceneItem(IDevice device)
        {
            DeviceUuid = device.Uuid;
            DeviceFamily = device.Family;
        }

        public List<Color> Colors { get; set; }
        public string DeviceFamily { get; set; }
        public string DeviceUuid { get; set; }
        public Effect Effect { get; set; }
        public PowerState Power { get; set; }
        public Theme Theme { get; set; }
    }
}
