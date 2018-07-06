using System;
using com.tibbo.aggregate.common.util;

namespace com.tibbo.aggregate.common.datatable.validator
{
    public abstract class AbstractRecordValidator : RecordValidator, ICloneable
    {
        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is RecordValidator))
            {
                return false;
            }

            return Util.equals(getType(), ((RecordValidator) obj).getType());
        }

        public Object Clone()
        {
            return MemberwiseClone();
        }

        public abstract Char? getType();
        public abstract String encode();
        public abstract void validate(DataTable table, DataRecord record);

        public override int GetHashCode()
        {
            return 0;
        }
    }
}