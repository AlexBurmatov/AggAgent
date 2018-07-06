using System;

namespace com.tibbo.aggregate.common.context
{
    public class FireEventRequestData : RequestData
    {
        private Int64? customExpirationPeriod; // Milliseconds, NULL if not defined

        public FireEventRequestData(Int64? customExpirationPeriod)
        {
            this.customExpirationPeriod = customExpirationPeriod;
        }

        public Int64? getCustomExpirationPeriod()
        {
            return customExpirationPeriod;
        }

        public void setCustomExpirationPeriod(Int64? customExpirationPeriodLong)
        {
            customExpirationPeriod = customExpirationPeriodLong;
        }
    }
}