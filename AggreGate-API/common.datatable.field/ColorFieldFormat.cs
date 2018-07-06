using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using com.tibbo.aggregate.common.util;

namespace com.tibbo.aggregate.common.datatable.field
{
    public class ColorFieldFormat : FieldFormat
    {
        public const String EDITOR_BOX = "box";

        public ColorFieldFormat(String name) : base(name)
        {
        }

        public override char getType()
        {
            return COLOR_FIELD;
        }

        public override Type getFieldClass()
        {
            return typeof (Color);
        }

        public override Type getFieldWrappedClass()
        {
            return typeof (Color);
        }

        public override object getNotNullDefault()
        {
            return Color.Black;
        }

        public override object valueFromString(string value, ClassicEncodingSettings settings, bool validate)
        {
            if (value.StartsWith("#") && value.Length == 7)
            {
                var red = int.Parse(value.Substring(1, 3), NumberStyles.AllowHexSpecifier);
                var green = int.Parse(value.Substring(3, 5), NumberStyles.AllowHexSpecifier);
                var blue = int.Parse(value.Substring(5, 7), NumberStyles.AllowHexSpecifier);
                return Color.FromArgb(red, green, blue);
            }
            return Color.FromArgb(int.Parse(value));
        }

        public override string valueToString(object value, ClassicEncodingSettings settings)
        {
            var correctValue = (Color?)value;

            return StringUtils.colorToString(correctValue);
        }

        protected override List<string> getSuitableEditors()
        {
            return new List<string>(new[] { EDITOR_BOX });
        }
    }
}