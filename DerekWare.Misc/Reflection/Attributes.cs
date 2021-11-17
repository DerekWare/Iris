using System;

namespace DerekWare.Reflection
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class AliasAttribute : Attribute
    {
        public string Alias;

        public AliasAttribute(string alias)
        {
            Alias = alias;
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class NameAttribute : Attribute
    {
        public string Name;

        public NameAttribute(string name)
        {
            Name = name;
        }
    }
}
