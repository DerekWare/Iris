#define UseParseMethods

using System;
using System.Collections.Generic;
using System.ComponentModel;
using DerekWare.Collections;
using DerekWare.Strings;

namespace DerekWare.Reflection
{
    public class TypeConverter
    {
        public readonly Type SourceType;
        public readonly Type TargetType;

        protected readonly Reflector Reflector;
        protected readonly System.ComponentModel.TypeConverter SourceConverter;
        protected readonly System.ComponentModel.TypeConverter TargetConverter;

        public TypeConverter(Type sourceType, Type targetType)
        {
            SourceType = sourceType ?? typeof(object);
            TargetType = targetType ?? typeof(object);

            while(SourceType.IsByRef || SourceType.IsPointer)
            {
                SourceType = SourceType.GetElementType();
            }

            while(TargetType.IsByRef || TargetType.IsPointer)
            {
                TargetType = TargetType.GetElementType();
            }

            SourceConverter = TypeDescriptor.GetConverter(SourceType);
            TargetConverter = TypeDescriptor.GetConverter(TargetType);
            Reflector = new Reflector(TargetType);
        }

        public virtual bool TryConvert(object source, out object result, out Exception exception)
        {
            result = null;
            exception = null;

            // Try a direct typecast
            if(TargetType.IsAssignableFrom(SourceType))
            {
                result = source;
                return true;
            }

            // Special-case string
            // TODO this works well if the type has a Parse method for converting back from a string,
            // but may not be the best idea if we have to go through IConvertible. The string may
            // contain necessary information that's not in ToString.
            var sourceString = source.SafeToString();

            if(typeof(string) == TargetType)
            {
                result = sourceString;
                return true;
            }

            // Handle null
            if((null == source) || (source is string && sourceString.IsNullOrEmpty()))
            {
                if(TargetType.IsClass)
                {
                    return true;
                }

                // TODO use default() instead?
                exception = new NullReferenceException($"Can't convert null to non-nullable type {TargetType}", exception);
                return false;
            }

            // Try to find a direct converter. If that doesn't work, try to convert through a string
            try
            {
                if(SourceConverter.CanConvertTo(TargetType))
                {
                    result = SourceConverter.ConvertTo(source, TargetType);
                    return true;
                }
            }
            catch(Exception ex)
            {
                exception = ex;
            }

            try
            {
                if(TargetConverter.CanConvertFrom(SourceType))
                {
                    result = TargetConverter.ConvertFrom(source);
                    return true;
                }
            }
            catch(Exception ex)
            {
                exception = ex;
            }

            try
            {
                if(TargetConverter.CanConvertFrom(typeof(string)))
                {
                    result = TargetConverter.ConvertFromString(sourceString);
                    return true;
                }
            }
            catch(Exception ex)
            {
                exception = ex;
            }

#if UseParseMethods

            // Try to parse the string version through reflection. We'll first use TryParse and Parse 
            // methods, then fall back on a constructor. This adds more flexibility to the method 
            // signatures of Parse and TryParse than I assume TypeConverter uses.
            // TODO how redundant is this with TypeConverter?
            // TODO find extension methods
            var args = new object[] { sourceString, true, null };
            var method = Reflector.GetMember("TryParse".AsEnumerable(), typeof(bool), args);

            try
            {
                if((null != method) && (bool)method.Evaluate(args))
                {
                    result = args[2];
                    return true;
                }
            }
            catch(Exception ex)
            {
                exception = ex;
            }

            args = new object[] { sourceString, null };
            method = Reflector.GetMember("TryParse".AsEnumerable(), typeof(bool), args);

            try
            {
                if((null != method) && (bool)method.Evaluate(args))
                {
                    result = args[1];
                    return true;
                }
            }
            catch(Exception ex)
            {
                exception = ex;
            }

            args = new object[] { sourceString, true };
            method = Reflector.GetMember(Enumerable.AsEnumerable("Parse", ".ctor"), TargetType, args);

            try
            {
                if(null != method)
                {
                    result = method.Evaluate(args);
                    return true;
                }
            }
            catch(Exception ex)
            {
                exception = ex;
            }

            args = new object[] { sourceString };
            method = Reflector.GetMember(Enumerable.AsEnumerable("Parse", ".ctor"), TargetType, args);

            try
            {
                if(null != method)
                {
                    result = method.Evaluate(args);
                    return true;
                }
            }
            catch(Exception ex)
            {
                exception = ex;
            }

#endif

            exception = new NullReferenceException($"No converter or parser for {SourceType} to {TargetType} exists", exception);
            return false;
        }
    }

    public static class TypeConversion
    {
        #region Conversion

        public static object Convert(this object source, Type targetType)
        {
            if(!TryConvert(source, targetType, out var result, out var exception))
            {
                throw exception;
            }

            return result;
        }

        public static TTarget Convert<TTarget>(this object source)
        {
            return (TTarget)Convert(source, typeof(TTarget));
        }

        public static object Parse(this string source, Type targetType)
        {
            return Convert(source, targetType);
        }

        public static TTarget Parse<TTarget>(this string source)
        {
            return Convert<TTarget>(source);
        }

        #endregion

        public static int? CompareAs<TTarget>(object x, object y, IComparer<TTarget> comparer = null)
        {
            comparer = comparer ?? Comparer<TTarget>.Default;

            if(!TryConvert(x, out TTarget a) || !TryConvert(y, out TTarget b))
            {
                return null;
            }

            return comparer.Compare(a, b);
        }

        public static int CompareAsStandardTypes(object x, object y, StringComparison comparisonType = StringComparison.Ordinal)
        {
            {
                if(double.TryParse(x.SafeToString(), out var a) && double.TryParse(y.SafeToString(), out var b))
                {
                    return Comparer<double>.Default.Compare(a, b);
                }
            }

            {
                if(bool.TryParse(x.SafeToString(), out var a) && bool.TryParse(y.SafeToString(), out var b))
                {
                    return Comparer<bool>.Default.Compare(a, b);
                }
            }

            return string.Compare(x.SafeToString(), y.SafeToString(), comparisonType);
        }

        public static bool? EqualsAs<TTarget>(object x, object y, IEqualityComparer<TTarget> comparer = null)
        {
            comparer = comparer ?? EqualityComparer<TTarget>.Default;

            if(!TryConvert(x, out TTarget a) || !TryConvert(y, out TTarget b))
            {
                return null;
            }

            return comparer.Equals(a, b);
        }

        public static bool TryConvert(this object source, Type targetType, out object result, out Exception exception)
        {
            return new TypeConverter(source?.GetType(), targetType).TryConvert(source, out result, out exception);
        }

        public static bool TryConvert(this object source, Type targetType, out object result)
        {
            return TryConvert(source, targetType, out result, out var exception);
        }

        public static bool TryConvert<T>(this object source, out T result, out Exception exception)
        {
            if(TryConvert(source, typeof(T), out var _result, out exception))
            {
                result = (T)_result;
                return true;
            }

            result = default;
            return false;
        }

        public static bool TryConvert<T>(this object source, out T result)
        {
            return TryConvert(source, out result, out var exception);
        }

        public static T TryConvert<T>(this object source)
        {
            if(TryConvert(source, out T result))
            {
                return result;
            }

            return default;
        }

        public static bool TryParse(this string source, Type targetType, out object result, out Exception exception)
        {
            return TryConvert(source, targetType, out result, out exception);
        }

        public static bool TryParse(this string source, Type targetType, out object result)
        {
            return TryConvert(source, targetType, out result);
        }

        public static bool TryParse<T>(this string source, out T result, out Exception exception)
        {
            return TryConvert(source, out result, out exception);
        }

        public static bool TryParse<T>(this string source, out T result)
        {
            return TryConvert(source, out result);
        }

        public static T TryParse<T>(this string source)
        {
            return TryConvert<T>(source);
        }
    }
}
