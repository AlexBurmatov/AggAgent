using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.tibbo.aggregate.common.context;

namespace com.tibbo.aggregate.common.@event
{
    using com.tibbo.aggregate.common.data;

    public class FireEventRequestController : DefaultRequestController
    {
        private long? customExpirationPeriod;

        private bool ignoreStorageErrors;

        public FireEventRequestController(long? customExpirationPeriodLong)
        {
            this.customExpirationPeriod = customExpirationPeriodLong;
        }

        public FireEventRequestController(bool ignoreStorageErrorsBool)
        {
            this.ignoreStorageErrors = ignoreStorageErrorsBool;
        }

        public long? getCustomExpirationPeriod()
        {
            return this.customExpirationPeriod;
        }

        public void setCustomExpirationPeriod(long? customExpirationPeriodLong)
        {
            this.customExpirationPeriod = customExpirationPeriodLong;
        }

        public bool isIgnoreStorageErrors()
        {
            return this.ignoreStorageErrors;
        }

        public void setIgnoreStorageErrors(bool ignoreStorageErrorsBool)
        {
            this.ignoreStorageErrors = ignoreStorageErrorsBool;
        }

        public virtual Event process(Event anEvent)
        {
            return anEvent;
        }


    }
}
