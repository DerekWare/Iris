using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using DerekWare.Collections;

namespace DerekWare.IO.Serialization
{
    public static class XmlExtensions
    {
        public static IEnumerable<XmlElement> GetElements(this XmlNodeList @this)
        {
            return @this.OfType<XmlElement>();
        }

        public static IEnumerable<XmlElement> GetElements(this XmlNodeList @this, string name, StringComparer comparer = null)
        {
            return GetElements(@this).WhereEquals(e => e.Name, name, comparer ?? StringComparer.OrdinalIgnoreCase);
        }
    }
}
