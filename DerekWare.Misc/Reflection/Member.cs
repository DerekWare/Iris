using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DerekWare.Collections;
using Enumerable = System.Linq.Enumerable;

namespace DerekWare.Reflection
{
    public class Member
    {
        public readonly MemberInfo MemberInfo;
        public readonly Reflector Reflector;

        public Member(Reflector reflector, MemberInfo memberInfo)
        {
            Reflector = reflector;
            MemberInfo = memberInfo;
        }

        public virtual IEnumerable<Type> ArgumentTypes => Enumerable.Empty<Type>();
        public IEnumerable<CustomAttributeData> CustomAttributes => MemberInfo.CustomAttributes;
        public Type DeclaringType => MemberInfo.DeclaringType;
        public MemberTypes MemberType => MemberInfo.MemberType;
        public string Name => MemberInfo.Name;
        public Type ReflectedType => MemberInfo.ReflectedType;
        public virtual Type ReturnType => null;

        public virtual object Evaluate(object[] args = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(bool inherit = true)
            where TAttribute : Attribute
        {
            return MemberInfo.GetCustomAttributes<TAttribute>(inherit);
        }

        /// <summary>
        ///     Returns a set of distinct object descriptions, including any DescriptionAttributes and the object's ToString
        ///     result.
        /// </summary>
        public IEnumerable<string> GetObjectDescription(bool inherit = true)
        {
            return GetCustomAttributes<DescriptionAttribute>(inherit).Select(v => v.Description).Append(Name).WhereNotNull().Distinct();
        }

        /// <summary>
        ///     Returns a set of distinct object names, including the object's Name member (if it exists), its ToString
        ///     result and any NameAttributes or AliasAttributes.
        /// </summary>
        public IEnumerable<string> GetObjectName(bool inherit = true)
        {
            return GetCustomAttributes<NameAttribute>(inherit)
                   .Select(v => v.Name)
                   .Append(GetCustomAttributes<AliasAttribute>(inherit).Select(v => v.Alias))
                   .Append(Name)
                   .WhereNotNull()
                   .Distinct();
        }

        public override string ToString()
        {
            return MemberInfo.Name;
        }

        public static implicit operator MemberInfo(Member obj)
        {
            return obj?.MemberInfo;
        }

        public static T[] ConvertArgumentTypes<T>(object[] args)
        {
            if((null == args) || (args.Length <= 0))
            {
                return null;
            }

            var result = new T[args.Length];

            for(var i = 0; i < result.Length; ++i)
            {
                result[i] = args[i].Convert<T>();
            }

            return result;
        }

        public static object[] ConvertArgumentTypes(object[] args, Type[] targetTypes)
        {
            if((null == args) || (args.Length <= 0))
            {
                return null;
            }

            if(args.Length != targetTypes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(args), "Argument count mismatch");
            }

            var result = new object[args.Length];

            for(var i = 0; i < result.Length; ++i)
            {
                result[i] = args[i]?.Convert(targetTypes[i]);
            }

            return result;
        }
    }
}
