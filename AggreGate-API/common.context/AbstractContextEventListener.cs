using System;
using com.tibbo.aggregate.common.data;

namespace com.tibbo.aggregate.common.context
{
    public abstract class AbstractContextEventListener<T> : ContextEventListener where T : CallerController<CallerData>
    {
        protected readonly T callerController;
        protected readonly Int32? listenerCode;

        protected AbstractContextEventListener()
        {
        }

        protected AbstractContextEventListener(T callerController)
        {
            this.callerController = callerController;
        }


        protected AbstractContextEventListener(T callerController, Int32? listenerCode) : this(callerController)
        {
            this.listenerCode = listenerCode;
        }

        public virtual Boolean shouldHandle(Event ev)
        {
            return true;
        }

        public abstract void handle(Event anEvent);

        public virtual CallerController<CallerData> getCallerController()
        {
            return callerController;
        }

        public Int32? getListenerCode()
        {
            return listenerCode;
        }
    }
}