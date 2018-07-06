using System;
using System.Collections.Generic;

namespace com.tibbo.aggregate.common.datatable.field
{
    public class LongFieldFormat : FieldFormat
    {
        public const string EDITOR_PERIOD = "period";

        public LongFieldFormat(string name) : base(name)
        {
        }

        public override char getType()
        {
            return LONG_FIELD;
        }

        public override Type getFieldClass()
        {
            return typeof (long);
        }

        public override Type getFieldWrappedClass()
        {
            return typeof (long);
        }

        public override object getNotNullDefault()
        {
            return 0;
        }

        public override object valueFromString(string value, ClassicEncodingSettings settings, bool validate)
        {
            try
            {
                return long.Parse(value);
            }
            catch (FormatException)
            {
                return (long)float.Parse(value);
            }
        }

        public override string valueToString(object value, ClassicEncodingSettings settings)
        {
            return value == null ? null : value.ToString();
        }


        protected override List<string> getSuitableEditors()
        {
            return new List<string>(new[] {EDITOR_PERIOD});
        }

        public static string encodePeriodEditorOptions(int minUnit, int maxUnit)
        {
            return minUnit + " " + maxUnit;
        }
    }
}