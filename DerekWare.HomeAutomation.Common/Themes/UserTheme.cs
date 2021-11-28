using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using DerekWare.HomeAutomation.Common.Colors;
using Newtonsoft.Json;

namespace DerekWare.HomeAutomation.Common.Themes
{
    [Browsable(false), Description("The user theme is created via a theme editor and saved in the application settings."), Serializable, JsonObject]
    public class UserTheme : Theme
    {
        public UserTheme()
        {
        }

        public UserTheme(SerializationInfo info, StreamingContext context)
        {
            this.Deserialize(info, context);
        }

        [Browsable(false)]
        public override bool IsDynamic => false;

        [Browsable(false)]
        public override bool IsMultiZone => Palette.Count > 1;

        public override string Name { get => base.Name; set => base.Name = value; }

        public List<Color> Palette { get; set; } = new();

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this.Serialize(info, context);
        }

        public override IReadOnlyCollection<Color> GetPalette(IDevice targetDevice)
        {
            return Palette;
        }
    }
}
