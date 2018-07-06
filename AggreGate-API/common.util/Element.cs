using System;
using com.tibbo.aggregate.common.datatable;

namespace com.tibbo.aggregate.common.util
{
    using System.Text;

    using command;

    public class Element : StringEncodable
    {
        private string name;
        private string value;
        private StringEncodable encodable;
        private FieldFormat fieldFormat;
        private object fieldValue;

        public Element(String name, String value)
        {
            this.name = name;
            this.value = value;
        }

        public Element(String name, StringEncodable encodable)
        {
            this.name = name;
            this.encodable = encodable;
        }

        public Element(String name, FieldFormat ff, Object fieldValue)
        {
            this.name = name;
            this.fieldFormat = ff;
            this.fieldValue = fieldValue;
        }

        public String getName()
        {
            return this.name;
        }


        public String getValue()
        {
            return this.value;
        }


        public String encode(ClassicEncodingSettings settings)
        {
            return this.encode(new StringBuilder(), settings, false, 0).ToString();
        }

        public StringBuilder encode(StringBuilder sb, ClassicEncodingSettings settings)
        {
            return this.encode(sb, settings, false, 0);
        }

        public StringBuilder encode(StringBuilder sb, ClassicEncodingSettings settings, int encodeLevel)
        {
            return this.encode(sb, settings, false, encodeLevel);
        }

        public StringBuilder encode(StringBuilder sb, ClassicEncodingSettings settings, bool isTransferEncode, int encodeLevel)
        {
            bool useVisibleSeparators = false;

            if (settings != null)
            {
                useVisibleSeparators = settings.isUseVisibleSeparators();
            }

            char elStart = useVisibleSeparators ? DataTableUtils.ELEMENT_VISIBLE_START : DataTableUtils.ELEMENT_START;
            char elEnd = useVisibleSeparators ? DataTableUtils.ELEMENT_VISIBLE_END : DataTableUtils.ELEMENT_END;
            char elNameValSep = useVisibleSeparators ? DataTableUtils.ELEMENT_VISIBLE_NAME_VALUE_SEPARATOR : DataTableUtils.ELEMENT_NAME_VALUE_SEPARATOR;

            if (isTransferEncode)
            {
                TransferEncodingHelper.encodeChar(elStart, sb);
                if (name != null)
                {
                    TransferEncodingHelper.encode(name, sb, 0);
                    TransferEncodingHelper.encodeChar(elNameValSep, sb);
                }
            }
            else
            {
                sb.Append(elStart);
                if (name != null)
                {
                    sb.Append(name);
                    sb.Append(elNameValSep);
                }
            }

            if (encodable != null)
            {
                encodable.encode(sb, settings, isTransferEncode, encodeLevel);
            }
            else if (fieldFormat != null)
            {
                fieldFormat.valueToEncodedString(fieldValue, settings, sb, encodeLevel);
            }
            else
            {
                if (isTransferEncode)
                    TransferEncodingHelper.encode(value, sb, encodeLevel);
                else
                    sb.Append(value);
            }

            if (isTransferEncode)
                TransferEncodingHelper.encodeChar(elEnd, sb);
            else
                sb.Append(elEnd);
            return sb;
        }

    }
}