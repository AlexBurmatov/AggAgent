using System;
using System.Collections.Generic;

namespace com.tibbo.aggregate.common.util
{
    public class TimeHelper
    {
        public const long SECOND_IN_MS = 1000;
        public const long MINUTE_IN_MS = SECOND_IN_MS*60;
        public const long HOUR_IN_MS = MINUTE_IN_MS*60;
        public const long DAY_IN_MS = HOUR_IN_MS*24;
        public const long WEEK_IN_MS = DAY_IN_MS*7;
        public const long MONTH_IN_MS = DAY_IN_MS*30;
        public const long QUARTER_IN_MS = DAY_IN_MS*91;
        public const long YEAR_IN_MS = DAY_IN_MS*365;

        public const long MINUTE_IN_SECONDS = 60;
        public const long HOUR_IN_SECONDS = MINUTE_IN_SECONDS*60;
        public const long DAY_IN_SECONDS = HOUR_IN_SECONDS*24;
        public const long WEEK_IN_SECONDS = DAY_IN_SECONDS*7;
        public const long MONTH_IN_SECONDS = DAY_IN_SECONDS*30;
        public const long QUARTER_IN_SECONDS = DAY_IN_SECONDS*91;
        public const long YEAR_IN_SECONDS = DAY_IN_SECONDS*365;

        public const int MILLISECOND = 0;
        public const int SECOND = 1;
        public const int MINUTE = 2;
        public const int HOUR = 3;
        public const int DAY = 4;
        public const int WEEK = 5;
        public const int MONTH = 6;
        public const int QUARTER = 7;
        public const int YEAR = 8;

        public static readonly TimeUnit MILLISECOND_UNIT = new TimeUnit(MILLISECOND, 1,
                                                                        Cres.get().getString("tuMilliseconds"));

        public static readonly TimeUnit SECOND_UNIT = new TimeUnit(SECOND, SECOND_IN_MS,
                                                                   Cres.get().getString("tuSeconds"));

        public static readonly TimeUnit MINUTE_UNIT = new TimeUnit(MINUTE, MINUTE_IN_MS,
                                                                   Cres.get().getString("tuMinutes"));

        public static readonly TimeUnit HOUR_UNIT = new TimeUnit(HOUR, HOUR_IN_MS, Cres.get().getString("tuHours"));
        public static readonly TimeUnit DAY_UNIT = new TimeUnit(DAY, DAY_IN_MS, Cres.get().getString("tuDays"));
        public static readonly TimeUnit WEEK_UNIT = new TimeUnit(WEEK, WEEK_IN_MS, Cres.get().getString("tuWeeks"));
        public static readonly TimeUnit MONTH_UNIT = new TimeUnit(MONTH, MONTH_IN_MS, Cres.get().getString("tuMonths"));

        public static readonly TimeUnit QUARTER_UNIT = new TimeUnit(QUARTER, QUARTER_IN_MS,
                                                                    Cres.get().getString("tuQuarters"));

        public static readonly TimeUnit YEAR_UNIT = new TimeUnit(YEAR, YEAR_IN_MS, Cres.get().getString("tuYears"));

        private static readonly Dictionary<object, string> SELECTION_VALUES = new Dictionary<Object, String>();
        private static readonly List<TimeUnit> UNITS = new List<TimeUnit>();

        static TimeHelper()
        {
            SELECTION_VALUES.Add(MILLISECOND, Cres.get().getString("tuMillisecond"));
            SELECTION_VALUES.Add(SECOND, Cres.get().getString("tuSecond"));
            SELECTION_VALUES.Add(MINUTE, Cres.get().getString("tuMinute"));
            SELECTION_VALUES.Add(HOUR, Cres.get().getString("tuHour"));
            SELECTION_VALUES.Add(DAY, Cres.get().getString("tuDay"));
            SELECTION_VALUES.Add(WEEK, Cres.get().getString("tuWeek"));
            SELECTION_VALUES.Add(MONTH, Cres.get().getString("tuMonth"));
            SELECTION_VALUES.Add(QUARTER, Cres.get().getString("tuQuarter"));
            SELECTION_VALUES.Add(YEAR, Cres.get().getString("tuYear"));

            UNITS.Add(MILLISECOND_UNIT);
            UNITS.Add(SECOND_UNIT);
            UNITS.Add(MINUTE_UNIT);
            UNITS.Add(HOUR_UNIT);
            UNITS.Add(DAY_UNIT);
            UNITS.Add(WEEK_UNIT);
            UNITS.Add(MONTH_UNIT);
            UNITS.Add(QUARTER_UNIT);
            UNITS.Add(YEAR_UNIT);
        }

        public static Dictionary<Object, String> getSelectionValues()
        {
            return SELECTION_VALUES;
        }

        public static List<TimeUnit> getUnits()
        {
            return UNITS;
        }

        public static String getUnitDescription(int unit)
        {
            return SELECTION_VALUES[unit];
        }

        public static TimeUnit getTimeUnit(int unit)
        {
            foreach (TimeUnit tu in UNITS)
            {
                if (tu.getUnit() == unit)
                {
                    return tu;
                }
            }

            throw new InvalidOperationException("Unknown time unit: " + unit);
        }

        public static long convertToMillis(long period, int unit)
        {
            switch (unit)
            {
                case MILLISECOND:
                    return period;

                case SECOND:
                    return period*SECOND_IN_MS;

                case MINUTE:
                    return period*MINUTE_IN_MS;

                case HOUR:
                    return period*HOUR_IN_MS;

                case DAY:
                    return period*DAY_IN_MS;

                case WEEK:
                    return period*WEEK_IN_MS;

                case MONTH:
                    return period*MONTH_IN_MS;

                case QUARTER:
                    return period*QUARTER_IN_MS;

                case YEAR:
                    return period*YEAR_IN_MS;

                default:
                    throw new InvalidOperationException("Unknown time unit: " + unit);
            }
        }
    }
}