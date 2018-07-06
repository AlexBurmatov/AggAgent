using System;
using System.Globalization;
using System.Text;

namespace com.tibbo.aggregate.common.datatable.field
{
    using com.tibbo.aggregate.common.util;

    public class FloatFieldFormat : FieldFormat
    {
        public FloatFieldFormat(String name) : base(name)
        {
        }

        public override char getType()
        {
            return FLOAT_FIELD;
        }

        public override Type getFieldClass()
        {
            return typeof(float);
        }

        public override Type getFieldWrappedClass()
        {
            return typeof(float);
        }

        public override object getNotNullDefault()
        {
            return 0.0f;
        }

        public override object valueFromString(String value, ClassicEncodingSettings settings, Boolean validate)
        {
            var nf = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
            nf.NumberDecimalSeparator = ".";
            return float.Parse(value, nf);
        }

        public override string valueToString(object value, ClassicEncodingSettings settings)
        {
            var nf = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
            nf.NumberDecimalSeparator = ".";
            return string.Format(nf, "{0}", value);
        }

    }
}