using System;
using System.Collections.Generic;
using System.Globalization;
using JavaCompatibility;

namespace com.tibbo.aggregate.common.datatable.field
{
    using com.tibbo.aggregate.common.util;

    public class DateFieldFormat : FieldFormat
    {
        public const String EDITOR_TIME_ONLY = "time";
        public const String EDITOR_DATE_ONLY = "date";

        public const string FORMAT = "yyyy-MM-dd HH:mm:ss.fff";

        private static readonly DateTime DEFAULT_DATE;

        static DateFieldFormat()
        {
            try
            {
                DEFAULT_DATE = new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            }
            catch (Exception ex)
            {
                Logger.getLogger(Log.DATATABLE).error("Error initializing default date", ex);
            }
        }

        [NonSerialized]
        private readonly DateTimeFormatInfo dateFormatter;

        public DateFieldFormat(string name) : base(name)
        {
            dateFormatter = new DateTimeFormatInfo();
            dateFormatter.SetAllDateTimePatterns(new[] { FORMAT }, 'd');
        }

        public override char getType()
        {
            return DATE_FIELD;
        }

        public override Type getFieldClass()
        {
            return typeof(DateTime);
        }

        public override Type getFieldWrappedClass()
        {
            return typeof(DateTime);
        }

        public override object getNotNullDefault()
        {
            return DEFAULT_DATE;
        }

        public override object valueFromString(String value, ClassicEncodingSettings settings, Boolean validate)
        {
            try
            {
                return DateTime.Parse(value, dateFormatter);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error parsing date from string '" + value + "': " + ex.Message, ex);
            }
        }

        public override string valueToString(object value, ClassicEncodingSettings settings)
        {
            return ((DateTime)value).ToString(FORMAT);
        }


        protected override List<string> getSuitableEditors()
        {
            return new List<string>(new[] { EDITOR_DATE_ONLY, EDITOR_TIME_ONLY });
        }
    }
}