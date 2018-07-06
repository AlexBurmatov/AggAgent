using System;

namespace com.tibbo.aggregate.common.datatable.validator
{
    public interface TableValidator
    {
        Char? getType();

        String encode();

        void validate(DataTable table);
    }
}