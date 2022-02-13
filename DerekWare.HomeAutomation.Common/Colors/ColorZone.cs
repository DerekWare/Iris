using System;
using System.Collections.Generic;
using DerekWare.Reflection;
using Newtonsoft.Json.Linq;

namespace DerekWare.HomeAutomation.Common
{
    public sealed class ColorZone : ICloneable<ColorZone>, IEquatable<ColorZone>, IComparable<ColorZone>, IComparable
    {
        public Color Color { get; set; } = new();
        public int EndIndex { get; set; }
        public int Size { get => (EndIndex - StartIndex) + 1; set => EndIndex = (StartIndex + value) - 1; }
        public int StartIndex { get; set; }

        public bool OverlapsWith(ColorZone other)
        {
            return Overlaps(this, other);
        }

        public override string ToString()
        {
            return new JObject(new JProperty(nameof(StartIndex), StartIndex), new JProperty(nameof(EndIndex), EndIndex), new JProperty(nameof(Color), Color))
                .ToString();
        }

        #region Equality

        public bool Equals(ColorZone other)
        {
            if(ReferenceEquals(null, other))
            {
                return false;
            }

            if(ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(Color, other.Color) && (EndIndex == other.EndIndex) && (StartIndex == other.StartIndex);
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

            return Equals((ColorZone)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Color != null ? Color.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ EndIndex;
                hashCode = (hashCode * 397) ^ StartIndex;
                return hashCode;
            }
        }

        public static bool operator ==(ColorZone left, ColorZone right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ColorZone left, ColorZone right)
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

        #region ICloneable<ColorZone>

        public ColorZone Clone()
        {
            return Reflection.Clone(this);
        }

        #endregion

        #region IComparable

        public int CompareTo(object obj)
        {
            if(ReferenceEquals(null, obj))
            {
                return 1;
            }

            if(ReferenceEquals(this, obj))
            {
                return 0;
            }

            return obj is ColorZone other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(ColorZone)}");
        }

        #endregion

        #region IComparable<ColorZone>

        public int CompareTo(ColorZone other)
        {
            if(ReferenceEquals(this, other))
            {
                return 0;
            }

            if(ReferenceEquals(null, other))
            {
                return 1;
            }

            var c = StartIndex.CompareTo(other.StartIndex);

            if(c == 0)
            {
                c = EndIndex.CompareTo(other.EndIndex);
            }

            return c;
        }

        #endregion

        public static bool operator >(ColorZone left, ColorZone right)
        {
            return Comparer<ColorZone>.Default.Compare(left, right) > 0;
        }

        public static bool operator >=(ColorZone left, ColorZone right)
        {
            return Comparer<ColorZone>.Default.Compare(left, right) >= 0;
        }

        public static bool operator <(ColorZone left, ColorZone right)
        {
            return Comparer<ColorZone>.Default.Compare(left, right) < 0;
        }

        public static bool operator <=(ColorZone left, ColorZone right)
        {
            return Comparer<ColorZone>.Default.Compare(left, right) <= 0;
        }

        public static bool Overlaps(ColorZone x, ColorZone y)
        {
            if(x is null || y is null)
            {
                return false;
            }

            return x.StartIndex <= y.StartIndex ? x.EndIndex >= y.StartIndex : y.EndIndex >= x.StartIndex;
        }
    }
}
