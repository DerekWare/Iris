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
            if(value is Color)
            {
                return value;
            }

            var sourceType = value?.GetType();

            if(typeof(System.Drawing.Color) == sourceType)
            {
                return new Color((System.Drawing.Color)value);
            }

            if(typeof(string) == sourceType)
            {
                var valueString = value.ToString();
                var field = typeof(Colors).GetField(valueString, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static);
                return field is not null ? field.GetValue(null) : Color.Parse(valueString);
            }

            throw new NotSupportedException();
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if(value is not Color color)
            {
                throw new NotSupportedException();
            }

            if(destinationType == typeof(Color))
            {
                return value;
            }

            if(typeof(System.Drawing.Color) == destinationType)
            {
                return color.ToRgb();
            }

            if(typeof(string) == destinationType)
            {
                var field = typeof(Colors).GetFields(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static)
                                          .FirstOrDefault(field => field.GetValue(null).Equals(color));
                return field is not null ? field.Name : color.ToString();
            }

            throw new NotSupportedException();
        }
    }
}
