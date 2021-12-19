using System;
using System.Collections.Generic;
using System.Linq;
using DerekWare.Collections;

namespace DerekWare.Misc.Schedule
{
    public abstract class TimeEvent : ITimeEvent
    {
        protected TimeSpan[] _Times = Array.Empty<TimeSpan>();

        public virtual DateTime Next
        {
            get
            {
                var next = DateTime.Now;

                while(!GetNextTime(ref next))
                {
                    next = AdvanceDate(next);
                }

                return next;
            }
        }

        public virtual IReadOnlyCollection<TimeSpan> Times { get => _Times; set => _Times = value.SafeEmpty().OrderBy(i => i).ToArray(); }

        protected virtual DateTime AdvanceDate(DateTime next)
        {
            return next.Date.AddDays(1);
        }

        protected virtual bool GetNextTime(ref DateTime next)
        {
            // Find a time today
            foreach(var time in Times)
            {
                if(time < next.TimeOfDay)
                {
                    continue;
                }

                next = next.Date + time;
                return true;
            }

            // Nope. Maybe Tomorrow.
            return false;
        }
    }
}
