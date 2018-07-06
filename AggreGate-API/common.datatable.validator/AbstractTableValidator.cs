using System;
using com.tibbo.aggregate.common.util;

namespace com.tibbo.aggregate.common.datatable.validator
{
    public abstract class AbstractTableValidator : TableValidator, ICloneable
    {
        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is TableValidator))
            {
                return false;
            }

            return Util.equals(getType(), ((TableValidator) obj).getType());
        }

        public Object Clone()
        {
            return MemberwiseClone();
        }

        public virtual char? getType()
        {
            throw new NotImplementedException();
        }

        public virtual string encode()
        {
            throw new NotImplementedException();
        }

        public virtual void validate(DataTable table)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}