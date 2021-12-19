using System;

namespace DerekWare.Misc.Schedule
{
    public static class Extensions
    {
        public static DateTime FirstDayOfMonth(this DateTime now)
        {
            return now.AddDays(-now.Day);
        }

        public static DateTime FirstDayOfWeek(this DateTime now)
        {
            return now.AddDays(-(int)now.DayOfWeek);
        }
    }
}
