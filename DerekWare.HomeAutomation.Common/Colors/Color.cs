using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using DerekWare.Diagnostics;
using DerekWare.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DerekWare.HomeAutomation.Common
{
    [TypeConverter(typeof(ColorConverter)), Serializable, JsonObject]
    public sealed class Color : ICloneable<Color>, IEquatable<Color>, ISerializable
    {
        double _Hue, _Saturation, _Brightness, _Kelvin;

        public Color()
        {
        }

        public Color(Color src)
        {
            Hue = src.Hue;
            Saturation = src.Saturation;
            Brightness = src.Brightness;
            Kelvin = src.Kelvin;
        }

        public Color(System.Drawing.Color src)
        {
            src.RgbToHsv(out var h, out var s, out var v);

            Hue = h / 360;
            Saturation = s;
            Brightness = Math.Max(v * 2, 1);
        }

        public Color(double h, double s, double b, double k)
        {
            Hue = h;
            Saturation = s;
            Brightness = b;
            Kelvin = k;
        }

        public Color(SerializationInfo info, StreamingContext context)
        {
            this.Deserialize(info, context);
        }

        public bool IsWhite => Saturation <= double.Epsilon;
        public string Name => this.GetColorName();

        public double Brightness
        {
            get => _Brightness;
            set
            {
                Debug.Assert(value is >= 0 and <= 1);
                _Brightness = value.Clamp(0, 1);
            }
        }

        public double Hue
        {
            get => _Hue;
            set
            {
                Debug.Assert(value is >= 0 and <= 1);
                _Hue = value.Clamp(0, 1);
            }
        }

        public double Kelvin
        {
            get => _Kelvin;
            set
            {
                Debug.Assert(value is >= 0 and <= 1);
                _Kelvin = value.Clamp(0, 1);
            }
        }

        public double Saturation
        {
            get => _Saturation;
            set
            {
                Debug.Assert(value is >= 0 and <= 1);
                _Saturation = value.Clamp(0, 1);
            }
        }

        public System.Drawing.Color ToRgb()
        {
            return Colors.HsvToRgb(Hue * 360, Saturation, Brightness);
        }

        public override string ToString()
        {
            return Name ??
                   new JObject(new JProperty(nameof(Hue), Hue),
                               new JProperty(nameof(Saturation), Saturation),
                               new JProperty(nameof(Brightness), Brightness),
                               new JProperty(nameof(Kelvin), Kelvin)).ToString();
        }

        #region Conversion

        public static Color Parse(string text)
        {
            var c = Colors.GetColorByName(text);

            if(c is not null)
            {
                return c;
            }

            var j = JObject.Parse(text);
            return new Color(j[nameof(Hue)].Value<double>(),
                             j[nameof(Saturation)].Value<double>(),
                             j[nameof(Brightness)].Value<double>(),
                             j[nameof(Kelvin)].Value<double>());
        }

        #endregion

        #region Equality

        public bool Equals(Color other)
        {
            if(ReferenceEquals(null, other))
            {
                return false;
            }

            if(ReferenceEquals(this, other))
            {
                return true;
            }

            return Brightness.Equals(other.Brightness) && Hue.Equals(other.Hue) && Kelvin.Equals(other.Kelvin) && Saturation.Equals(other.Saturation);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj))
            {
                return false;
            }

            if(ReferenceEquals(this, obj))
            {
                return true;
            }

            if(obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Color)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Brightness.GetHashCode();
                hashCode = (hashCode * 397) ^ Hue.GetHashCode();
                hashCode = (hashCode * 397) ^ Kelvin.GetHashCode();
                hashCode = (hashCode * 397) ^ Saturation.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Color left, Color right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Color left, Color right)
        {
            return !Equals(left, right);
        }

        #endregion

        #region ICloneable

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

        #region ICloneable<Color>

        public Color Clone()
        {
            return new Color(this);
        }

        #endregion

        #region ISerializable

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this.Serialize(info, context);
        }

        #endregion

        public static Color operator +(Color src, double value)
        {
            return new Color(src.Hue + value, src.Saturation + value, src.Brightness + value, src.Kelvin + value);
        }

        public static Color operator +(Color x, Color y)
        {
            return new Color(x.Hue + y.Hue, x.Saturation + y.Saturation, x.Brightness + y.Brightness, x.Kelvin + y.Kelvin);
        }

        public static Color operator *(Color src, double value)
        {
            return new Color(src.Hue * value, src.Saturation * value, src.Brightness * value, src.Kelvin * value);
        }

        public static Color operator *(Color x, Color y)
        {
            return new Color(x.Hue * y.Hue, x.Saturation * y.Saturation, x.Brightness * y.Brightness, x.Kelvin * y.Kelvin);
        }

        public static Color operator -(Color src, double value)
        {
            return new Color(src.Hue - value, src.Saturation - value, src.Brightness - value, src.Kelvin - value);
        }

        public static Color operator -(Color x, Color y)
        {
            return new Color(x.Hue - y.Hue, x.Saturation - y.Saturation, x.Brightness - y.Brightness, x.Kelvin - y.Kelvin);
        }

        public static bool TryParse(string text, out Color color)
        {
            try
            {
                color = Parse(text);
                return true;
            }
            catch(Exception ex)
            {
                Debug.Warning(null, ex);
                color = default;
                return false;
            }
        }
    }
}
