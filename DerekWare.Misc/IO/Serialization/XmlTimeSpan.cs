using System;
using System.Xml.Serialization;

namespace DerekWare.IO.Serialization
{
    // The TimeSpan structure isn't serializable to XML or JSON, so use this helper class
    // by way of the XmlElement(Type) attribute.
    // 
    // [XmlElement(Type = typeof(XmlTimeSpan))]
    // public TimeSpan TimeSinceThisWorked { get; set; }
    public class XmlTimeSpan
    {
        TimeSpan Value = TimeSpan.Zero;

        public XmlTimeSpan()
        {
        }

        public XmlTimeSpan(TimeSpan source)
        {
            Value = source;
        }

        [XmlText]
        public string Default { get => Value.ToString(); set => Value = TimeSpan.Parse(value); }

        public static implicit operator TimeSpan?(XmlTimeSpan o)
        {
            return o?.Value;
        }

        public static implicit operator XmlTimeSpan(TimeSpan? o)
        {
            return o == null ? null : new XmlTimeSpan(o.Value);
        }

        public static implicit operator TimeSpan(XmlTimeSpan o)
        {
            return o?.Value ?? default;
        }

        public static implicit operator XmlTimeSpan(TimeSpan o)
        {
            return o == default ? null : new XmlTimeSpan(o);
        }
    }
}
