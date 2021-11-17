using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DerekWare.Reflection
{
    public class Field : Member
    {
        public readonly FieldInfo FieldInfo;

        public Field(Reflector reflector, FieldInfo fieldInfo)
            : base(reflector, fieldInfo)
        {
            FieldInfo = fieldInfo;
        }

        public override IEnumerable<Type> ArgumentTypes
        {
            get
            {
                if(!FieldInfo.FieldType.IsArray)
                {
                    return base.ArgumentTypes;
                }

                return Enumerable.Repeat(typeof(int), FieldInfo.FieldType.GetArrayRank());
            }
        }

        public override Type ReturnType
        {
            get
            {
                var type = FieldInfo.FieldType;

                while(type.IsArray)
                {
                    type = type.GetElementType();
                }

                return type;
            }
        }

        public override object Evaluate(object[] args = null)
        {
            var _args = ConvertArgumentTypes<int>(args);
            var result = FieldInfo.GetValue(Reflector.Instance);

            if((null != args) && (args.Length > 0))
            {
                result = ((Array)result).GetValue(_args);
            }

            return result;
        }

        public static implicit operator FieldInfo(Field other)
        {
            return other.FieldInfo;
        }
    }
}
