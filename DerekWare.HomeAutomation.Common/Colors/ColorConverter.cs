using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

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
            if(value is Color or null)
            {
                return value;
            }

            var sourceType = value.GetType();

            if(typeof(System.Drawing.Color) == sourceType)
            {
                return new Color((System.Drawing.Color)value);
            }

            if(typeof(string) == sourceType)
            {
                return Color.Parse((string)value);
            }

            throw new NotSupportedException();
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if(value is not Color color)
            {
                throw new NotSupportedException();
            }

            if(typeof(Color) == destinationType)
            {
                return value;
            }

            if(typeof(System.Drawing.Color) == destinationType)
            {
                return color.ToRgb();
            }

            if(typeof(string) == destinationType)
            {
                return color.ToString();
            }

            throw new NotSupportedException();
        }
    }
}
