using System;
using com.tibbo.aggregate.common.datatable;

namespace com.tibbo.aggregate.datatable.converter
{
    public class AbstractFormatConverter : FormatConverter<object>
    {
        private Type valueClass; 
        private readonly TableFormat format;

        public AbstractFormatConverter(Type valueClass, TableFormat format)
        {
            this.valueClass = valueClass;
            this.format = format;
        }

        public AbstractFormatConverter(Type valueClass)
        {
            this.valueClass = valueClass;
        }

        public Type getValueClass()
        {
            return this.valueClass;
        }

        public void setValueClass(Type valueClassType)
        {
            this.valueClass = valueClassType;
        }

        public TableFormat getFormat()
        { 
            return format;
        }

        public virtual FieldFormat createFieldFormat(string name)
        {
            return DataTableUtils.createTableField(name, format);
        }
    }
}