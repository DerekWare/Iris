using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace DerekWare.HomeAutomation.Common.Colors
{
    public class ColorConverter : TypeConverter
    {
        static readonly Type[] SupportedTypes = { typeof(System.Drawing.Color), typeof(string) };

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return SupportedTypes.Contains(sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return SupportedTypes.Contains(destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var sourceType = value?.GetType();

            if(typeof(System.Drawing.Color) == sourceType)
            {
                return new Color((System.Drawing.Color)value);
            }

            if(typeof(string) == sourceType)
            {
                var field = typeof(Colors).GetField(value.ToString(), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static);

                if(field is not null)
                {
                    return field.GetValue(null);
                }
            }

            throw new NotSupportedException();
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if(value is not Color color)
            {
                throw new NotSupportedException();
            }

            if(typeof(System.Drawing.Color) == destinationType)
            {
                return color.ToRgb();
            }

            if(typeof(string) == destinationType)
            {
                var field = typeof(Colors).GetFields(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static)
                                          .FirstOrDefault(field => field.GetValue(null).Equals(color));

                if(field is not null)
                {
                    return field.Name;
                }
            }

            throw new NotSupportedException();
        }
    }
}
