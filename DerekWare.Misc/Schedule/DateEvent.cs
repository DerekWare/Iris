using System;
using System.Collections.Generic;
using System.Linq;
using DerekWare.Collections;

namespace DerekWare.Misc.Schedule
{
    public abstract class DateEvent : TimeEvent, IDateEvent
    {
        public abstract int MaxDateValue { get; }

        protected abstract bool GetNextDate(ref DateTime next);

        protected int[] _Dates = Array.Empty<int>();

        public virtual int MinDateValue => 0;

        public override DateTime Next
        {
            get
            {
                var now = DateTime.Now;
                var next = now;

                while(true)
                {
                    // Try to find a date within our range (week, month, etc). If none is found,
                    // advance to the next week/month/whatever.
                    if(!GetNextDate(ref next))
                    {
                        next = AdvanceDate(next);
                        continue;
                    }

                    // Try to find a time within that date. If none is found (which should only
                    // happen if the date is today), advance by 1 day and start over.
                    if(!GetNextTime(ref next))
                    {
                        next = base.AdvanceDate(next);
                        continue;
                    }

                    break;
                }

                return next;
            }
        }

        public virtual IReadOnlyCollection<int> Dates
        {
            get => _Dates;
            set
            {
                var days = value.SafeEmpty().OrderBy(i => i).ToArray();

                if(days.Any(day => (day < MinDateValue) || (day > MaxDateValue)))
                {
                    throw new IndexOutOfRangeException("Invalid date");
                }

                _Dates = days;
            }
        }
    }
}
