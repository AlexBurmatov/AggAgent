using System;
using com.tibbo.aggregate.common.datatable;
using com.tibbo.aggregate.datatable.converter;
using JavaCompatibility;

namespace com.tibbo.aggregate.common.@event
{
    public class Acknowledgement : Object, ICloneable
    {
        public static readonly TableFormat FORMAT = new TableFormat();

        static Acknowledgement()
        {
            FORMAT.addField("<author><S>");
            FORMAT.addField("<time><D>");
            FORMAT.addField("<text><S>");

            DataTableUtils.registerFormatConverter(new DefaultFormatConverter(typeof (Acknowledgement), FORMAT));
        }

        private String author;
        private DateTime time;
        private String text;

        public Acknowledgement()
        {
        }

        public Acknowledgement(String authorString, DateTime aDateTime, String textString)
        {
            author = authorString;
            time = aDateTime;
            text = textString;
        }

        public String getAuthor()
        {
            return author;
        }

        public String getText()
        {
            return text;
        }

        public DateTime getTime()
        {
            return time;
        }

        public void setAuthor(String authorString)
        {
            author = authorString;
        }

        public void setText(String textString)
        {
            text = textString;
        }

        public void setTime(DateTime aDateTime)
        {
            time = aDateTime;
        }

        public TableFormat getFormat()
        {
            return FORMAT;
        }

        public Object Clone()
        {
            try
            {
                return MemberwiseClone();
            }
            catch (CloneNotSupportedException ex)
            {
                throw new InvalidOperationException("", ex);
            }
        }
    }
}