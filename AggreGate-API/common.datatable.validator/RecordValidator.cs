using System;

namespace com.tibbo.aggregate.common.datatable.validator
{
    public interface RecordValidator
    {
        Char? getType();

        String encode();

        void validate(DataTable table, DataRecord record);
    }
}