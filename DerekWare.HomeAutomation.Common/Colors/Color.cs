using System;
using System.ComponentModel;
using DerekWare.Diagnostics;
using DerekWare.Reflection;

namespace DerekWare.HomeAutomation.Common.Colors
{
    [TypeConverter(typeof(ColorConverter))]
    public sealed class Color : ICloneable<Color>, IEquatable<Color>
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

        public bool IsWhite => Saturation == 0;

        public double Brightness
        {
            get => _Brightness;
            set
            {
                Debug.Assert(value is >= 0 and <= 1);
                _Brightness = Math.Min(Math.Max(value, 0), 1);
            }
        }

        public double Hue
        {
            get => _Hue;
            set
            {
                Debug.Assert(value is >= 0 and <= 1);
                _Hue = Math.Min(Math.Max(value, 0), 1);
                ;
            }
        }

        public double Kelvin
        {
            get => _Kelvin;
            set
            {
                Debug.Assert(value is >= 0 and <= 1);
                _Kelvin = Math.Min(Math.Max(value, 0), 1);
                ;
            }
        }

        public double Saturation
        {
            get => _Saturation;
            set
            {
                Debug.Assert(value is >= 0 and <= 1);
                _Saturation = Math.Min(Math.Max(value, 0), 1);
                ;
            }
        }

        public System.Drawing.Color ToRgb()
        {
            return Extensions.HsvToRgb(Hue * 360, Saturation, Brightness);
        }

        public override string ToString()
        {
            return $"{{ Hue:{Hue}, Saturation:{Saturation}, Brightness:{Brightness}, Kelvin:{Kelvin} }}";
        }

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
    }
}
