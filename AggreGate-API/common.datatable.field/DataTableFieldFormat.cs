using System;
using com.tibbo.aggregate.common.datatable.validator;

namespace com.tibbo.aggregate.common.datatable.field
{
    using System.Text;

    using com.tibbo.aggregate.common.context;
    using com.tibbo.aggregate.common.util;

    public class DataTableFieldFormat : FieldFormat
    {
        public DataTableFieldFormat(String name) : base(name)
        {
            addValidator(new DataTableFieldValidator(this));
        }

        public override char getType()
        {
            return DATATABLE_FIELD;
        }

        public override Type getFieldClass()
        {
            return typeof(DataTable);
        }

        public override Type getFieldWrappedClass()
        {
            return typeof(DataTable);
        }

        public override object getNotNullDefault()
        {
            return new DataTable();
        }

        public override object valueFromString(String value, ClassicEncodingSettings settings, Boolean validate)
        {
            try
            {
                DataTable defaultValue = (DataTable)getDefaultValue();

                bool? tempEncodeFieldNames = null;
                TableFormat oldFormat = null;
                if (settings != null)
                {
                    oldFormat = settings.getFormat();
                    if (defaultValue != null)
                    {
                        settings.setFormat(defaultValue.getFormat());
                    }
                    tempEncodeFieldNames = settings.isEncodeFieldNames();
                }
                else
                {
                    settings = new ClassicEncodingSettings(false);
                    tempEncodeFieldNames = false;
                }

                try
                {
                    DataTable res = value != null ? new DataTable(value, settings, validate) : null;
                    // TODO: add data replication and validation

                    //                  if (defaultValue != null && defaultValue.getFieldCount() > 0 && !res.getFormat().extend(defaultValue.getFormat()))
                    //                    {
                    //                       DataTable newRes = (DataTable) defaultValue.Clone();
                    //                       DataTableReplication.copy(res, newRes, true, true, true);
                    //                    }

                    //                    if (res != null && validate)
                    //                    {
                    //                        res.validate(null, null, null);
                    //                    }

                    return res;
                }
                finally
                {
                    settings.setFormat(oldFormat);
                    settings.setEncodeFieldNames((bool)tempEncodeFieldNames);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error constructing value of field '" + ToString() + "': " + ex.Message, ex);
            }
        }

        public override string valueToString(object value, ClassicEncodingSettings settings)
        {

            if (value == null)
            {
                return null;
            }

            var correctValue = (DataTable)value;

            var oldEncodeFormat = settings != null ? (bool?) settings.isEncodeFormat() : null;

            try
            {
                if (settings != null && (getDefaultValue() == null || ((DataTable)getDefaultValue()).getFieldCount() == 0 || !Util.equals(((DataTable)getDefaultValue()).getFormat(), correctValue.getFormat())))
                {
                    settings.setEncodeFormat(true);
                }

                return correctValue.encode(settings);
            }
            finally
            {
                if (oldEncodeFormat != null)
                {
                    settings.setEncodeFormat((bool) oldEncodeFormat);
                }
            }
        }

        public class DataTableFieldValidator : AbstractFieldValidator
        {
            public DataTableFieldValidator(DataTableFieldFormat aDataTableFieldFormat)
            {
                this.ownerFormat = aDataTableFieldFormat;
            }

            public override object validate(Context context, ContextManager contextManager, CallerController<CallerData> caller, object value)
            {
                DataTable def = (DataTable)this.ownerFormat.getDefaultValue();

                if (def == null || def.getFieldCount() == 0)
                {
                    return value;
                }

                DataTable dt = (DataTable)value;

                if (dt == null)
                {
                    return value;
                }

                string msg = dt.getFormat().extendMessage(def.getFormat());
                if (msg != null)
                {
                    DataTable newValue = def.Clone() as DataTable;

                    // TODO: add data replication and validation
                    //DataTableReplication.copy(dt, newValue, true, true, true);
                    //value = newValue;

                    throw new NotImplementedException("DataTable copy is not implemented yet");
                }

                return value;
            }

            private DataTableFieldFormat ownerFormat;

        }
    }
}
