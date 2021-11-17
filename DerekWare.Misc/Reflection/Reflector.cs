using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DerekWare.Collections;
using DerekWare.Strings;
using Enumerable = DerekWare.Collections.Enumerable;

namespace DerekWare.Reflection
{
    /// <summary>
    ///     Provides various helper methods for reflection into an object or type.
    /// </summary>
    public class Reflector
    {
        public const BindingFlags DefaultBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

        public static readonly ValueMap<Type, Type> MemberInstanceTypes = new ValueMap<Type, Type>
        {
            { typeof(FieldInfo), typeof(Field) },
            { typeof(PropertyInfo), typeof(Property) },
            { typeof(MethodInfo), typeof(Method) },
            { typeof(MemberInfo), typeof(Member) }
        };

        public readonly BindingFlags BindingFlags;
        public readonly object Instance;
        public readonly Type Type;

        List<Member> _Members;

        /// <summary>
        ///     Instantiates a Reflector for a given type.
        /// </summary>
        public Reflector(Type type, BindingFlags bindingFlags = DefaultBindingFlags)
        {
            BindingFlags = bindingFlags;
            Type = type;
        }

        /// <summary>
        ///     Instantiates a Reflector for a given object.
        /// </summary>
        public Reflector(object obj, BindingFlags bindingFlags = DefaultBindingFlags)
            : this(obj as Type, bindingFlags)
        {
            if(!(Type is null))
            {
                return;
            }

            Type = obj.GetType();
            Instance = obj;
        }

        /// <summary>
        ///     Returns all members for the object or type.
        /// </summary>
        public IReadOnlyList<Member> Members => _Members ?? (_Members = Type.GetMembers().Select(CreateMemberInstance).ToList());

        /// <summary>
        ///     Returns all custom attributes of the given attribute type.
        /// </summary>
        public IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(bool inherit = true)
            where TAttribute : Attribute
        {
            if(Instance is null)
            {
                return Type.GetCustomAttributes<TAttribute>(inherit);
            }

            return GetMember(Instance.ToString())?.GetCustomAttributes<TAttribute>(inherit) ?? Enumerable.Empty<TAttribute>();
        }

        /// <summary>
        ///     Finds all extension methods in a given assembly that take the Reflector's type as the first argument.
        /// </summary>
        public IEnumerable<Method> GetExtensionMethods(Assembly assembly)
        {
            return from assemblyType in assembly.GetTypes()
                   where assemblyType.IsSealed &&
                         !assemblyType.IsGenericType &&
                         !assemblyType.IsNested &&
                         assemblyType.IsDefined(typeof(ExtensionAttribute), false)
                   from method in assemblyType.GetMethods(BindingFlags)
                   where method.IsDefined(typeof(ExtensionAttribute), false)
                   let parameters = method.GetParameters()
                   where (parameters.Length >= 1) && parameters[0].ParameterType.IsAssignableFrom(Type)
                   select new Method(this, method);
        }

        /// <summary>
        ///     Finds all extension methods in all assemblies that take the Reflector's type as the first argument.
        /// </summary>
        public IEnumerable<Method> GetExtensionMethods()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(GetExtensionMethods);
        }

        /// <summary>
        ///     Evaluates the value of all fields.
        /// </summary>
        /// <returns>An enumerable of KeyValuePairs containing the field name and value.</returns>
        public IEnumerable<KeyValuePair<string, object>> GetFieldValues()
        {
            return GetFieldValues<object>();
        }

        /// <summary>
        ///     Evaluates the value of all fields.
        /// </summary>
        /// <returns>An enumerable of KeyValuePairs containing the field name and value.</returns>
        public IEnumerable<KeyValuePair<string, TValue>> GetFieldValues<TValue>()
        {
            return from member in GetMembers(null, typeof(TValue), Array.Empty<object>())
                   let value = member.Evaluate()
                   from name in member.GetObjectName()
                   select new KeyValuePair<string, TValue>(name, (TValue)value);
        }

        /// <summary>
        ///     Evaluates the value of all fields.
        /// </summary>
        /// <returns>A dictionary of KeyValuePairs containing the field name and value.</returns>
        public Dictionary<string, TValue> GetFieldValues<TValue>(IEqualityComparer<string> comparer)
        {
            return GetFieldValues<TValue>().ToDictionary(comparer);
        }

        /// <summary>
        ///     Gets a member matching the given name.
        /// </summary>
        /// <param name="memberName">The member name.</param>
        /// <param name="returnType">The member return type or null to ignore this parameter.</param>
        /// <param name="args">
        ///     Arguments for evaluating the member. GetMember will compare the types of each attribute to see if
        ///     they're appropriate for evaluating the member.
        /// </param>
        /// <param name="ignoreCase">True to ignore the case of the member name.</param>
        /// <returns>The found member or null.</returns>
        public Member GetMember(string memberName, Type returnType = null, object[] args = null, bool ignoreCase = true)
        {
            return GetMembers(memberName.AsEnumerable(), returnType, args, ignoreCase).FirstOrDefault();
        }

        /// <summary>
        ///     Gets a member matching the given name.
        /// </summary>
        /// <param name="memberName">The member name.</param>
        /// <param name="returnType">The member return type or null to ignore this parameter.</param>
        /// <param name="args">
        ///     Arguments for evaluating the member. GetMember will compare the types of each attribute to see if
        ///     they're appropriate for evaluating the member.
        /// </param>
        /// <param name="ignoreCase">True to ignore the case of the member name.</param>
        /// <returns>The found member or null.</returns>
        public TMember GetMember<TMember>(string memberName, Type returnType = null, object[] args = null, bool ignoreCase = true)
            where TMember : Member
        {
            return GetMembers(memberName.AsEnumerable(), returnType, args, ignoreCase).OfType<TMember>().FirstOrDefault();
        }

        /// <summary>
        ///     Gets a member matching any of the given names.
        /// </summary>
        /// <param name="memberNames">The member names.</param>
        /// <param name="returnType">The member return type or null to ignore this parameter.</param>
        /// <param name="args">
        ///     Arguments for evaluating the member. GetMember will compare the types of each attribute to see if
        ///     they're appropriate for evaluating the member.
        /// </param>
        /// <param name="ignoreCase">True to ignore the case of the member name.</param>
        /// <returns>The found member or null.</returns>
        public Member GetMember(IEnumerable<string> memberNames, Type returnType = null, object[] args = null, bool ignoreCase = true)
        {
            return GetMembers(memberNames, returnType, args, ignoreCase).FirstOrDefault();
        }

        /// <summary>
        ///     Gets a member matching any of the given names.
        /// </summary>
        /// <param name="memberNames">The member names.</param>
        /// <param name="returnType">The member return type or null to ignore this parameter.</param>
        /// <param name="args">
        ///     Arguments for evaluating the member. GetMember will compare the types of each attribute to see if
        ///     they're appropriate for evaluating the member.
        /// </param>
        /// <param name="ignoreCase">True to ignore the case of the member name.</param>
        /// <returns>The found member or null.</returns>
        public TMember GetMember<TMember>(IEnumerable<string> memberNames, Type returnType = null, object[] args = null, bool ignoreCase = true)
            where TMember : Member
        {
            return GetMembers(memberNames, returnType, args, ignoreCase).OfType<TMember>().FirstOrDefault();
        }

        /// <summary>
        ///     Gets all members matching any of the given names.
        /// </summary>
        /// <param name="memberNames">The member names.</param>
        /// <param name="returnType">The member return type or null to ignore this parameter.</param>
        /// <param name="args">
        ///     Arguments for evaluating the member. GetMember will compare the types of each attribute to see if
        ///     they're appropriate for evaluating the member.
        /// </param>
        /// <param name="ignoreCase">True to ignore the case of the member name.</param>
        /// <returns>The found members.</returns>
        public IEnumerable<Member> GetMembers(IEnumerable<string> memberNames, Type returnType = null, object[] args = null, bool ignoreCase = true)
        {
            var stringComparer = ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
            var predicates = new List<Func<Member, bool>>();

            if(!(memberNames is null))
            {
                predicates.Add(member => memberNames.Contains(member.Name, stringComparer));
            }

            if(!(returnType is null))
            {
                predicates.Add(member => returnType.IsAssignableFrom(member.ReturnType));
            }

            if(!(args is null))
            {
                predicates.Add(member =>
                {
                    var memberTypes = member.ArgumentTypes.SafeEmpty().ToArray();

                    if(memberTypes.Length != args.Length)
                    {
                        return false;
                    }

                    for(var i = 0; i < args.Length; ++i)
                    {
                        if(!args[i].TryConvert(memberTypes[i], out var result))
                        {
                            return false;
                        }
                    }

                    return true;
                });
            }

            return Members.Where(member => predicates.All(predicate => predicate(member)));
        }

        /// <summary>
        ///     Returns a set of distinct object descriptions, including any DescriptionAttributes and the object's ToString()
        ///     result.
        /// </summary>
        public IEnumerable<string> GetObjectDescription(bool inherit = true)
        {
            return GetCustomAttributes<DescriptionAttribute>(inherit).Select(v => v.Description).Append(Instance.SafeToString()).WhereNotNull().Distinct();
        }

        /// <summary>
        ///     Returns a set of distinct object names, including the object's Name member, its ToString() result and any
        ///     NameAttributes.
        /// </summary>
        public IEnumerable<string> GetObjectName(bool inherit = true)
        {
            return GetMember("Name", typeof(string), Array.Empty<object>())
                   ?.Evaluate()
                   .SafeToString()
                   .AsEnumerable()
                   .Append(GetCustomAttributes<NameAttribute>(inherit).Select(v => v.Name))
                   .Append(Instance.SafeToString())
                   .WhereNotNull()
                   .Distinct();
        }

        Member CreateMemberInstance(MemberInfo memberInfo)
        {
            var memberType = memberInfo.GetType();
            var instanceType = MemberInstanceTypes.First(i => i.Key.IsAssignableFrom(memberType)).Value;

            return (Member)Activator.CreateInstance(instanceType, this, memberInfo);
        }
    }
}
