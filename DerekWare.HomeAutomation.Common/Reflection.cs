using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DerekWare.Collections;
using DerekWare.Diagnostics;
using DerekWare.Reflection;

namespace DerekWare.HomeAutomation.Common
{
    public static class Reflection
    {
        public static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return from p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                   where p.CanRead
                   let b = p.GetCustomAttributes<BrowsableAttribute>(true).ToList()
                   where (b.Count <= 0) || b.First().Browsable
                   select p;
        }

        public static string GetTypeName(this Type type)
        {
            return type.GetCustomAttributes<NameAttribute>(false).FirstOrDefault()?.Name ?? type.Name;
        }

        public static IEnumerable<Type> GetVisibleTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(i => i.GetVisibleTypes());
        }

        public static IEnumerable<Type> GetVisibleTypes(this Assembly assembly)
        {
            return from t in assembly.GetTypes()
                   where !t.IsAbstract && t.IsVisible
                   let b = t.GetCustomAttributes<BrowsableAttribute>(false).FirstOrDefault()
                   where b is null || b.Browsable
                   select t;
        }

        public static IEnumerable<PropertyInfo> GetWritableProperties(Type type)
        {
            return from p in GetProperties(type)
                   where p.CanWrite
                   select p;
        }

        public static IReadOnlyDictionary<string, object> GetWritablePropertyValues(this object @this, Type type = null)
        {
            type ??= @this.GetType();

            return (from p in GetWritableProperties(type)
                    select new KeyValuePair<string, object>(p.Name, p.GetValue(@this))).ToDictionary();
        }

        public static bool SetPropertyValue(this object @this, string name, object value, Type type = null)
        {
            type ??= @this.GetType();

            var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);

            if(property is null || !property.CanWrite)
            {
                return false;
            }

            try
            {
                property.SetValue(@this, value);
            }
            catch(Exception e)
            {
                Debug.Warning(@this, e);
                return false;
            }

            return true;
        }

        public static int SetPropertyValues(this object @this, IReadOnlyDictionary<string, object> src, Type type = null)
        {
            return src.Sum(i => SetPropertyValue(@this, i.Key, i.Value, type) ? 1 : 0);
        }
    }
}
