using System;

namespace com.tibbo.aggregate.common.util
{
    public class TimeUnit
    {
        private readonly Int32 unit;
        private readonly Int64 length;
        private readonly String description;

        public TimeUnit(Int32 unit, Int64 length, String description)
        {
            this.unit = unit;
            this.length = length;
            this.description = description;
        }

        public Int32 getUnit()
        {
            return unit;
        }

        public Int64 getLength()
        {
            return length;
        }

        public String getDescription()
        {
            return description;
        }

        public override String ToString()
        {
            return description;
        }

        public override Int32 GetHashCode()
        {
            const Int32 prime = 31;
            var result = 1;
            result = prime*result + unit;
            return result;
        }

        public override Boolean Equals(Object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (GetType() != obj.GetType())
            {
                return false;
            }
            var other = (TimeUnit) obj;
            return unit == other.unit;
        }
    }
}