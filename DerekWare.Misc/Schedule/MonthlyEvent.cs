using System;

namespace DerekWare.Misc.Schedule
{
    public class MonthlyEvent : DateEvent, IMonthlyEvent
    {
        public override int MaxDateValue => 31;

        protected override DateTime AdvanceDate(DateTime date)
        {
            return Next.Date.FirstDayOfMonth().AddMonths(1);
        }

        protected override bool GetNextDate(ref DateTime next)
        {
            // Find the next day this month.
            foreach(var i in Dates)
            {
                var date = Math.Min(i, DateTime.DaysInMonth(next.Year, next.Month));

                if(date < next.Day)
                {
                    continue;
                }

                next = next.Date.FirstDayOfMonth().AddDays(date);
                return true;
            }

            // Nope. Next month.
            return false;
        }
    }
}
