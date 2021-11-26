using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DerekWare.HomeAutomation.Common
{
    class WritablePropertiesOnlyResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, memberSerialization).Where(IsWritable).ToList();
        }

        public static bool IsVisible(JsonProperty propertyInfo)
        {
            var attr = propertyInfo.AttributeProvider.GetAttributes(typeof(BrowsableAttribute), true).Cast<BrowsableAttribute>().ToList();
            return (attr.Count <= 0) || attr.First().Browsable;
        }

        public static bool IsWritable(JsonProperty propertyInfo)
        {
            return IsVisible(propertyInfo) && propertyInfo.Writable;
        }
    }

    public static class JsonSerializer
    {
        static readonly Newtonsoft.Json.JsonSerializer Serializer = new()
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new WritablePropertiesOnlyResolver()
        };

        public static T Deserialize<T>(string text)
        {
            using var stringReader = new StringReader(text);
            using var jsonReader = new JsonTextReader(stringReader);
            return Serializer.Deserialize<T>(jsonReader);
        }

        public static string Serialize(object obj)
        {
            using var stringWriter = new StringWriter();
            using var jsonWriter = new JsonTextWriter(stringWriter);
            Serializer.Serialize(jsonWriter, obj);
            return stringWriter.ToString();
        }
    }
}
