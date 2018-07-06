using System;
using System.Collections.Generic;

namespace com.tibbo.aggregate.common.datatable.validator
{
    public class TableKeyFieldsValidator : AbstractTableValidator
    {
        public override String encode()
        {
            return "";
        }

        public override Char? getType()
        {
            return TableFormat.TABLE_VALIDATOR_KEY_FIELDS;
        }

        public override void validate(DataTable table) 
        {
            foreach (var rec in table)
            {
                validate(table, rec);
            }
        }

        public void validate(DataTable table, DataRecord record) 
        {
            var keyFields = table.getFormat().getKeyFields();

            if (keyFields.Count == 0)
            {
                return;
            }

            var query = new Query();
            var key = new List<Object>();

            foreach (var keyField in keyFields)
            {
                var value = record.getValue(keyField);
                key.Add(value);
                query.addCondition(new QueryCondition(keyField, value));
            }

            var rec = table.select(query);

            if (rec != null && rec != record)
            {
                throw new ValidationException(String.Format(Cres.get().getString("dtKeyFieldViolation"), key));
            }
        }
    }
}