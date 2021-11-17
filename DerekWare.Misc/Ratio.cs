using System;

namespace DerekWare
{
    public struct Ratio : IEquatable<Ratio>
    {
        public int X;
        public int Y;

        public Ratio(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Ratio(Ratio other)
            : this(other.X, other.Y)
        {
        }

        public Ratio(string x, string y)
            : this(Parse(x, y))
        {
        }

        public Ratio(string value)
            : this(Parse(value))
        {
        }

        public bool IsEmpty => !IsValid;
        public bool IsValid => (X > 0) && (Y > 0);

        public void Reduce()
        {
            int x = X, y = Y, t = 1;

            while(y > 0)
            {
                t = y;
                y = x % y;
                x = t;
            }

            X /= t;
            Y /= t;
        }

        public double ToDouble()
        {
            return IsValid ? (double)X / Y : 0;
        }

        public override string ToString()
        {
            return $"{X}:{Y}";
        }

        #region Conversion

        public static Ratio Parse(string x, string y)
        {
            if(!TryParse(x, y, out var result))
            {
                throw new FormatException();
            }

            return result;
        }

        public static Ratio Parse(string source)
        {
            if(!TryParse(source, out var result))
            {
                throw new FormatException();
            }

            return result;
        }

        #endregion

        #region Equality

        public bool Equals(Ratio other)
        {
            return Equals(ToDouble(), other.ToDouble());
        }

        public override bool Equals(object obj)
        {
            return obj is Ratio && Equals((Ratio)obj);
        }

        public override int GetHashCode()
        {
            return ToDouble().GetHashCode();
        }

        public static bool operator ==(Ratio left, Ratio right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Ratio left, Ratio right)
        {
            return !left.Equals(right);
        }

        #endregion

        public static implicit operator double(Ratio r)
        {
            return r.ToDouble();
        }

        public static bool TryParse(string x, string y, out Ratio result)
        {
            if(int.TryParse(x, out var a) && int.TryParse(y, out var b))
            {
                result = new Ratio(a, b);
                return true;
            }

            result = default;
            return false;
        }

        public static bool TryParse(string source, out Ratio result)
        {
            var parts = source.Split(':');

            if(parts.Length == 2)
            {
                return TryParse(parts[0], parts[1], out result);
            }

            result = default;
            return false;
        }
    }
}
