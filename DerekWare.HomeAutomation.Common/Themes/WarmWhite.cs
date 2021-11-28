using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.Reflection;
using Newtonsoft.Json;

namespace DerekWare.HomeAutomation.Common.Themes
{
    [Name("Warm White"), Serializable, JsonObject]
    public class WarmWhite : Theme, ISerializable
    {
        public WarmWhite()
        {
        }

        public WarmWhite(SerializationInfo info, StreamingContext context)
        {
            this.Deserialize(info, context);
        }

        [Browsable(false)]
        public override bool IsDynamic => true;

        [Browsable(false)]
        public override bool IsMultiZone => false;

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public override IReadOnlyCollection<Color> GetPalette(IDevice targetDevice)
        {
            return new[] { Colors.Colors.WarmWhite };
        }

        #region ISerializable

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this.Serialize(info, context);
        }

        #endregion
    }
}
