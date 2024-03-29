﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Utility.Helper
{
    public static class DatetimeHelper
    {

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        public static IEnumerable<DateTime> EachMonth(DateTime from, DateTime thru)
        {
            for (var month = from.Date; month.Date <= thru.Date || month.Month == thru.Month; month = month.AddMonths(1))
                yield return month;
        }

        public static IEnumerable<DateTime> EachDayTo(this DateTime dateFrom, DateTime dateTo)
        {
            return EachDay(dateFrom, dateTo);
        }

        public static IEnumerable<DateTime> EachMonthTo(this DateTime dateFrom, DateTime dateTo)
        {
            return EachMonth(dateFrom, dateTo);
        }

        public static IEnumerable<DateTime> EachDateTime(DateTime from, DateTime thru)
        {
            for (var day = from; day.Date < thru.Date; day = day.AddDays(1))
                yield return day;
        }

        public static (DateTime? startDate, DateTime? endDate) DetaRangeConvert(this string dateRange, string format = "yyyy/MM/dd HH:mm")
        {
            DateTime _startDate;
            DateTime _endDate;

            if (string.IsNullOrEmpty(dateRange))
                return (null, null);

            string[] _dateAry = dateRange.Split("-");

            if (!DateTime.TryParseExact(_dateAry[0].Trim(), format, null, DateTimeStyles.None, out _) ||
                !DateTime.TryParseExact(_dateAry[1].Trim(), format, null, DateTimeStyles.None, out _))
            {
                return (null, null);
            }
            DateTime.TryParseExact(_dateAry[0].Trim(), format, null, DateTimeStyles.None, out _startDate);
            DateTime.TryParseExact(_dateAry[1].Trim(), format, null, DateTimeStyles.None, out _endDate);

            return (_startDate, _endDate);
        }
    }
}
