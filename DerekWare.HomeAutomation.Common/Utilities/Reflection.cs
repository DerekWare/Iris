using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DerekWare.Diagnostics;
using DerekWare.Reflection;
using Newtonsoft.Json;

namespace DerekWare.HomeAutomation.Common
{
    public static class Reflection
    {
        /// <summary>
        ///     Performs a deep copy of the object via reflection. All public, writable properties and
        ///     fields are copied, regardless of attributes. This can be implemented by a base type
        ///     as the reflection uses GetType, not typeof(T).
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="this">The object instance to copy.</param>
        /// <returns>A deep copy of the object.</returns>
        public static T Clone<T>(this T @this)
            where T : class
        {
            if(ReferenceEquals(@this, null))
            {
                return null;
            }

            var data = JsonSerializer.Serialize(@this);
            return JsonSerializer.Deserialize<T>(data);
        }

        public static string GetDescription(this Type type)
        {
            return type.GetCustomAttributes<DescriptionAttribute>(true).FirstOrDefault()?.Description;
        }

        public static string GetDescription(this object @this)
        {
            return GetDescription(@this.GetType());
        }

        public static string GetName(this Type type)
        {
            return type.GetCustomAttributes<NameAttribute>(true).FirstOrDefault()?.Name ?? type.Name;
        }

        public static string GetName(this object @this)
        {
            return GetName(@this.GetType());
        }

        public static IReadOnlyCollection<Assembly> GetReferencedAssemblies()
        {
            var remaining = new Queue<Assembly>();
            var found = new Dictionary<string, Assembly>();

            remaining.Enqueue(Assembly.GetEntryAssembly());

            while(remaining.Count > 0)
            {
                var nextAssembly = remaining.Dequeue();

                foreach(var reference in nextAssembly.GetReferencedAssemblies())
                {
                    if(found.ContainsKey(reference.FullName))
                    {
                        continue;
                    }

                    var assembly = Assembly.Load(reference);
                    remaining.Enqueue(assembly);
                    found.Add(reference.FullName, assembly);
                }
            }

            return found.Values;
        }

        public static IEnumerable<PropertyInfo> GetVisibleProperties(this object obj)
        {
            return GetVisibleProperties(obj.GetType());
        }

        public static IEnumerable<PropertyInfo> GetVisibleProperties(this Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(IsVisible);
        }

        public static IEnumerable<Type> GetVisibleTypes()
        {
            return GetReferencedAssemblies().SelectMany(GetVisibleTypes);
        }

        public static IEnumerable<Type> GetVisibleTypes(this Assembly assembly)
        {
            return assembly.GetTypes().Where(IsVisible);
        }

        public static IEnumerable<PropertyInfo> GetWritableProperties(this Type type)
        {
            return GetVisibleProperties(type).Where(p => p.CanWrite);
        }

        public static IEnumerable<PropertyInfo> GetWritableProperties(this object obj)
        {
            return GetWritableProperties(obj.GetType());
        }

        public static bool IsVisible(this Type type)
        {
            if(!type.IsVisible || type.IsAbstract)
            {
                return false;
            }

            var attr = type.GetCustomAttributes<BrowsableAttribute>(true).ToList();
            return (attr.Count <= 0) || attr.First().Browsable;
        }

        public static bool IsVisible(this PropertyInfo propertyInfo)
        {
            if(!propertyInfo.CanRead)
            {
                return false;
            }

            var attr = propertyInfo.GetCustomAttributes<BrowsableAttribute>(true).ToList();
            return (attr.Count <= 0) || attr.First().Browsable;
        }

        public static bool SetPropertyValue(this object @this, string name, object value, Type type = null)
        {
            type ??= @this.GetType();

            var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);

            if(property is null || !property.CanWrite)
            {
                return false;
            }

            if(!value.TryConvert(property.PropertyType, out value))
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
    }
}
