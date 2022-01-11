using System;
using System.Collections.Generic;
using DerekWare.Collections;

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

        public static bool GetBool()
        {
            return Base.Next(2) == 1;
        }

        public static void GetBytes(byte[] buffer)
        {
            Base.NextBytes(buffer);
        }

        public static double GetDouble(double minValue, double maxValue)
        {
            if(minValue > maxValue)
            {
                throw new IndexOutOfRangeException("minValue must be <= maxValue");
            }

            return (Base.NextDouble() * (maxValue - minValue)) + minValue;
        }

        public static double GetDouble(double maxValue)
        {
            return Base.NextDouble() * maxValue;
        }

        public static double GetDouble()
        {
            return Base.NextDouble();
        }

        public static T GetEnumValue<T>()
            where T : Enum
        {
            return GetItem<T>(typeof(T).GetEnumValues());
        }

        public static int GetInt()
        {
            return Base.Next();
        }

        public static int GetInt(int minValue, int maxValue)
        {
            if(minValue > maxValue)
            {
                throw new IndexOutOfRangeException("minValue must be <= maxValue");
            }

            return Base.Next(minValue, maxValue);
        }

        public static int GetInt(int maxValue)
        {
            return Base.Next(maxValue);
        }

        public static T GetItem<T>(IReadOnlyList<T> items)
        {
            if(items.IsNullOrEmpty())
            {
                throw new IndexOutOfRangeException("Empty collection");
            }

            return items[Base.Next(items.Count)];
        }

        public static T GetItem<T>(T[] items)
        {
            if(items.IsNullOrEmpty())
            {
                throw new IndexOutOfRangeException("Empty collection");
            }

            return items[Base.Next(items.Length)];
        }

        public static object GetItem(Array items)
        {
            if(items.IsNullOrEmpty())
            {
                throw new IndexOutOfRangeException("Empty collection");
            }

            return items.GetValue(Base.Next(items.Length));
        }

        public static T GetItem<T>(Array items)
        {
            if(items.IsNullOrEmpty())
            {
                throw new IndexOutOfRangeException("Empty collection");
            }

            return (T)GetItem(items);
        }

        public static TimeSpan GetTimeSpan(TimeSpan maxValue)
        {
            return TimeSpan.FromSeconds(GetDouble(maxValue.TotalSeconds));
        }

        public static TimeSpan GetTimeSpan(TimeSpan minValue, TimeSpan maxValue)
        {
            return TimeSpan.FromSeconds(GetDouble(minValue.TotalSeconds, maxValue.TotalSeconds));
        }
    }
}
