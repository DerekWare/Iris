using System;
using System.Runtime.CompilerServices;

namespace DerekWare
{
    public static partial class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(this int value, int min, int max)
        {
            return value < min ? min : value > max ? max : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Clamp(this long value, long min, long max)
        {
            return value < min ? min : value > max ? max : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Clamp(this uint value, uint min, uint max)
        {
            return value < min ? min : value > max ? max : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Clamp(this ulong value, ulong min, ulong max)
        {
            return value < min ? min : value > max ? max : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(this float value, float min, float max)
        {
            return value < min ? min : value > max ? max : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp(this double value, double min, double max)
        {
            return value < min ? min : value > max ? max : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short Clamp(this short value, short min, short max)
        {
            return value < min ? min : value > max ? max : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Clamp(this ushort value, ushort min, ushort max)
        {
            return value < min ? min : value > max ? max : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Clamp(this byte value, byte min, byte max)
        {
            return value < min ? min : value > max ? max : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char Clamp(this char value, char min, char max)
        {
            return value < min ? min : value > max ? max : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal Clamp(this decimal value, decimal min, decimal max)
        {
            return value < min ? min : value > max ? max : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetProgress(this TimeSpan value, TimeSpan range)
        {
            return GetProgress(value.Ticks, range.Ticks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetProgress(this double value, double range)
        {
            return value.IsNonZero() && range.IsNonZero() ? Math.Max(0, Math.Min(1, value / range)) : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNonZero(this double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value) && ((value >= double.Epsilon) || (value <= -double.Epsilon));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan Max(this TimeSpan a, TimeSpan b)
        {
            return a > b ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan Min(this TimeSpan a, TimeSpan b)
        {
            return a < b ? a : b;
        }
    }
}
