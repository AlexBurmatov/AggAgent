using System;
using com.tibbo.aggregate.common.datatable;

namespace com.tibbo.aggregate.datatable.converter
{
    public interface FormatConverter<T>
    {
        Type getValueClass();

        TableFormat getFormat();

        FieldFormat createFieldFormat(String name);
    }
}