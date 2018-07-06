using System;
using System.Text;

namespace com.tibbo.aggregate.common.datatable.field
{
    using com.tibbo.aggregate.common.util;

    public class BooleanFieldFormat : FieldFormat
    {
        public BooleanFieldFormat(string name) : base(name)
        {
        }

        public override char getType()
        {
            return BOOLEAN_FIELD;
        }

        public override Type getFieldClass()
        {
            return typeof (bool);
        }

        public override Type getFieldWrappedClass()
        {
            return typeof (bool);
        }

        public override object getNotNullDefault()
        {
            return false;
        }

        public override object valueFromString(String value, ClassicEncodingSettings settings, bool validate)
        {
            return value.Equals("1") || string.Compare(value, "true", true) == 0;
        }

        public override String valueToString(object value, ClassicEncodingSettings settings)
        {
            return (bool)value ? "1" : "0";
        }
    }
}