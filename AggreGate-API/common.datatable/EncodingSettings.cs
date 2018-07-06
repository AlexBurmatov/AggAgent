using System;

namespace com.tibbo.aggregate.common.datatable
{
    public class EncodingSettings
    {
        private bool encodeFormat;
        private TableFormat format;

        protected EncodingSettings(bool encodeFormat, TableFormat aTableFormat)
        {
            this.encodeFormat = encodeFormat;
            format = aTableFormat;
        }


        public TableFormat getFormat()
        {
            return format;
        }

        public void setFormat(TableFormat aTableFormat)
        {
            format = aTableFormat;
        }

        public bool isEncodeFormat()
        {
            return encodeFormat;
        }

        public void setEncodeFormat(bool encodeFormatBoolean)
        {
            encodeFormat = encodeFormatBoolean;
        }
    }
}