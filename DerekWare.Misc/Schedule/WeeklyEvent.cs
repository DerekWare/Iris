using System;

namespace DerekWare.Misc.Schedule
{
    public class WeeklyEvent : DateEvent, IWeeklyEvent
    {
        public override int MaxDateValue => 6;

        protected override DateTime AdvanceDate(DateTime date)
        {
            return date.Date.FirstDayOfWeek().AddDays(7);
        }

        protected override bool GetNextDate(ref DateTime next)
        {
            // Find the next day this week
            foreach(var date in Dates)
            {
                if(date < (int)next.DayOfWeek)
                {
                    continue;
                }

                next = next.Date.FirstDayOfWeek().AddDays(date);
                return true;
            }

            // Nope. Next week.
            return false;
        }
    }
}
