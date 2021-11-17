using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DerekWare.Reflection
{
    public class Method : Member
    {
        public readonly MethodInfo MethodInfo;

        public Method(Reflector reflector, MethodInfo methodInfo)
            : base(reflector, methodInfo)
        {
            MethodInfo = methodInfo;
        }

        public override IEnumerable<Type> ArgumentTypes => MethodInfo.GetParameters().Select(v => v.ParameterType);
        public override Type ReturnType => MethodInfo.ReturnType;

        public override object Evaluate(object[] args = null)
        {
            var _args = ConvertArgumentTypes(args, ArgumentTypes.ToArray());
            var result = MethodInfo.Invoke(Reflector.Instance, _args);

            if((null != _args) && (_args.Length > 0))
            {
                _args.CopyTo(args, 0);
            }

            return result;
        }

        public static implicit operator MethodInfo(Method other)
        {
            return other.MethodInfo;
        }
    }
}
