using System;
using System.Collections.Generic;

namespace DerekWare.Misc.Schedule
{
    public interface IDailyEvent : ITimeEvent
    {
    }

    public interface IDateEvent : IScheduledEvent
    {
        public IReadOnlyCollection<int> Dates { get; set; }
    }

    public interface IMonthlyEvent : IDateEvent, ITimeEvent
    {
    }

    public interface IScheduledEvent
    {
        public DateTime Next { get; }
    }

    public interface ITimeEvent : IScheduledEvent
    {
        public IReadOnlyCollection<TimeSpan> Times { get; set; }
    }

    public interface IWeeklyEvent : IDateEvent, ITimeEvent
    {
    }
}
