﻿using System;
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
        ///     Perform a deep copy of the object via reflection. All public, writable properties and
        ///     fields are copied, regardless of attributes. This can be implemented by a base type
        ///     as the reflection uses GetType, not typeof(T).
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="src">The object instance to copy.</param>
        /// <returns>A deep copy of the object.</returns>
        public static T Clone<T>(this T src)
            where T : class
        {
            if(ReferenceEquals(src, null))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(src,
                                                                                src.GetType(),
                                                                                new JsonSerializerSettings
                                                                                {
                                                                                    ObjectCreationHandling = ObjectCreationHandling.Replace
                                                                                }));
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

        public static IEnumerable<PropertyInfo> GetVisibleProperties(this Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(IsVisible);
        }

        public static IEnumerable<Type> GetVisibleTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(GetVisibleTypes);
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