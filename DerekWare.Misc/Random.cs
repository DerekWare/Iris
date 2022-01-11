using System;

namespace DerekWare
{
    /// <summary>
    ///     Provides convenience extensions and thread safety via TLS to System.Random.
    /// </summary>
    public static class Random
    {
        static readonly System.Random Global = new();

        [ThreadStatic]
        static System.Random Local;

        static System.Random Base
        {
            get
            {
                if(Local is null)
                {
                    int seed;

                    lock(Global)
                    {
                        seed = Global.Next();
                    }

                    Local = new System.Random(seed);
                }

                return Local;
            }
        }

        public static int Next()
        {
            return Base.Next();
        }

        public static int Next(int minValue, int maxValue)
        {
            if(minValue > maxValue)
            {
                throw new IndexOutOfRangeException("minValue must be <= maxValue");
            }

            return Base.Next(minValue, maxValue);
        }

        public static int Next(int maxValue)
        {
            return Base.Next(maxValue);
        }

        public static bool NextBool()
        {
            return Base.Next(2) == 1;
        }

        public static void NextBytes(byte[] buffer)
        {
            Base.NextBytes(buffer);
        }

        public static double NextDouble(double minValue, double maxValue)
        {
            if(minValue > maxValue)
            {
                throw new IndexOutOfRangeException("minValue must be <= maxValue");
            }

            return (Base.NextDouble() * (maxValue - minValue)) + minValue;
        }

        public static double NextDouble(double maxValue)
        {
            return Base.NextDouble() * maxValue;
        }

        public static double NextDouble()
        {
            return Base.NextDouble();
        }

        public static T NextEnumValue<T>()
            where T : Enum
        {
            var type = typeof(T);
            var values = type.GetEnumValues();
            var index = Base.Next(values.Length);
            return (T)values.GetValue(index);
        }

        public static TimeSpan NextTimeSpan(TimeSpan maxValue)
        {
            return TimeSpan.FromSeconds(NextDouble(maxValue.TotalSeconds));
        }

        public static TimeSpan NextTimeSpan(TimeSpan minValue, TimeSpan maxValue)
        {
            return TimeSpan.FromSeconds(NextDouble(minValue.TotalSeconds, maxValue.TotalSeconds));
        }
    }
}
