using System;
using System.Globalization;
using DerekWare.Strings;

namespace DerekWare
{
    public enum SimpleDateStyle
    {
        Full,
        Month,
        Year,
        Decade
    }

    public struct SimpleDate : IComparable<SimpleDate>, IComparable
    {
        public int? Day;
        public DayOfWeek? DayOfWeek;
        public int? Month;
        public int? Year;

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool abbreviate)
        {
            var result = "";

            if(DayOfWeek.HasValue)
            {
                result += abbreviate
                    ? DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(DayOfWeek.Value)
                    : DateTimeFormatInfo.CurrentInfo.GetDayName(DayOfWeek.Value);

                if(Month.HasValue || Day.HasValue || Year.HasValue)
                {
                    result += ", ";
                }
            }

            if(Month.HasValue)
            {
                result += abbreviate
                    ? DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(Month.Value)
                    : DateTimeFormatInfo.CurrentInfo.GetMonthName(Month.Value);
            }

            if(Day.HasValue)
            {
                if(!result.IsNullOrEmpty())
                {
                    result += " ";
                }

                result += Day.Value;
            }

            if(Year.HasValue)
            {
                if(!result.IsNullOrEmpty())
                {
                    result += ", ";
                }

                result += Year.Value;
            }

            return result;
        }

        #region IComparable

        public int CompareTo(object other)
        {
            if(other is SimpleDate sd)
            {
                return CompareTo(sd);
            }

            return -1;
        }

        #endregion

        #region IComparable<SimpleDate>

        public int CompareTo(SimpleDate other)
        {
            int c;

            if((c = Nullable.Compare(Year, other.Year)) != 0)
            {
                return c;
            }

            if((c = Nullable.Compare(Month, other.Month)) != 0)
            {
                return c;
            }

            return Nullable.Compare(Day, other.Day);
        }

        #endregion
    }

    public static partial class Extensions
    {
        public static SimpleDate ToSimpleDate(this DateTime that, SimpleDateStyle style = default)
        {
            switch(style)
            {
                case SimpleDateStyle.Full:
                    return new SimpleDate { Day = that.Day, DayOfWeek = that.DayOfWeek, Month = that.Month, Year = that.Year };

                case SimpleDateStyle.Month:
                    return new SimpleDate { Month = that.Month, Year = that.Year };

                case SimpleDateStyle.Year:
                    return new SimpleDate { Year = that.Year };

                case SimpleDateStyle.Decade:
                    return new SimpleDate { Year = (that.Year / 10) * 10 };

                default:
                    throw new ArgumentOutOfRangeException(nameof(style), style, null);
            }
        }
    }
}
