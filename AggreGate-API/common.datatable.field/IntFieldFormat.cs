using System;
using System.Collections.Generic;

namespace com.tibbo.aggregate.common.datatable.field
{
    public class IntFieldFormat : FieldFormat
    {
        public const string EDITOR_SPINNER = "spinner";

        public IntFieldFormat(string name) : base(name)
        {
        }

        public override char getType()
        {
            return INTEGER_FIELD;
        }

        public override Type getFieldClass()
        {
            return typeof(int);
        }

        public override Type getFieldWrappedClass()
        {
            return typeof(int);
        }

        public override object getNotNullDefault()
        {
            return 0;
        }

        public override object valueFromString(string value, ClassicEncodingSettings settings, bool validate)
        {
            if (value.Length == 0)
            {
                return 0;
            }
            try
            {
                return int.Parse(value);
            }
            catch (FormatException ex)
            {
                //return Util.convertToNumber(value, validate, false).intValue();
                return (int)float.Parse(value);
            }
        }

        public override string valueToString(object value, ClassicEncodingSettings settings)
        {
            return value.ToString();
        }

        protected override List<string> getSuitableEditors()
        {
            return new List<string>(new[] { EDITOR_SPINNER });
        }
    }
}