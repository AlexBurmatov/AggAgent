using System;
using System.Globalization;

namespace com.tibbo.aggregate.common.datatable.field
{
    using com.tibbo.aggregate.common.util;

    public class DoubleFieldFormat : FieldFormat
    {
        public DoubleFieldFormat(string name)
            : base(name)
        {
        }

        public override char getType()
        {
            return DOUBLE_FIELD;
        }

        public override Type getFieldClass()
        {
            return typeof(double);
        }

        public override Type getFieldWrappedClass()
        {
            return typeof(double);
        }

        public override object getNotNullDefault()
        {
            return 0.0d;
        }

        public override object valueFromString(string value, ClassicEncodingSettings settings, bool validate)
        {
            //var longValue = Int64.Parse(value);
            //var byteArray = BitConverter.GetBytes(longValue);
            //var stream = new MemoryStream(byteArray);
            //var reader = new BinaryReader(stream);
            //return reader.ReadDouble();

            var nf = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
            nf.NumberDecimalSeparator = ".";
            return double.Parse(value, nf);
        }

        public override string valueToString(object value, ClassicEncodingSettings settings)
        {
            //Double doubleValue = Convert.ToDouble(value);
            //byte[] bytes = new byte[8];
            //var stream = new MemoryStream(bytes);
            //new BinaryWriter(stream).Write(doubleValue);
            //Int64 longValue = BitConverter.ToInt64(bytes, 0);
            //var stringValue = longValue.ToString();
            //return stringValue;

            var nf = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
            nf.NumberDecimalSeparator = ".";
            return value == null ? null : String.Format(nf, "{0}", value);
        }

    }
}
