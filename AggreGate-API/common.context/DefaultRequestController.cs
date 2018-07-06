using System;

namespace com.tibbo.aggregate.common.context
{
    public class DefaultRequestController : RequestController<RequestData>
    {
        private object originator;
        private long? lockTimeout;
        // private Evaluator evaluator;
        // private String queue;

        public DefaultRequestController()
        {
        }

        public DefaultRequestController(object originator)
        {
            this.originator = originator;
        }

        public object getOriginator()
        {
            return this.originator;
        }


        public long? getLockTimeout()
        {
            return this.lockTimeout;
        }

        public void setLockTimeout(long? lockTimeoutLong)
        {
            this.lockTimeout = lockTimeoutLong;
        }

        public void setOriginator(object originatorObject)
        {
            this.originator = originatorObject;
        }
    }
}