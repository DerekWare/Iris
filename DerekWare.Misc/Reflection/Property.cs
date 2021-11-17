using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DerekWare.Reflection
{
    public class Property : Member
    {
        public readonly PropertyInfo PropertyInfo;

        public Property(Reflector reflector, PropertyInfo propertyInfo)
            : base(reflector, propertyInfo)
        {
            PropertyInfo = propertyInfo;
        }

        public override IEnumerable<Type> ArgumentTypes => PropertyInfo.GetIndexParameters().Select(v => v.ParameterType);
        public override Type ReturnType => PropertyInfo.PropertyType;

        public override object Evaluate(object[] args = null)
        {
            var _args = ConvertArgumentTypes(args, ArgumentTypes.ToArray());
            return PropertyInfo.GetValue(Reflector.Instance, _args);
        }

        public static implicit operator PropertyInfo(Property other)
        {
            return other.PropertyInfo;
        }
    }
}
