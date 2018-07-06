using System;
using com.tibbo.aggregate.common.data;

namespace com.tibbo.aggregate.common.context
{
    public class DefaultContextEventListener<T> : AbstractContextEventListener<T> where T : CallerController<CallerData>
    {
        public delegate void HandleDelegate(Event anEvent);

        public DefaultContextEventListener(HandleDelegate aHandler)
        {
            handler = aHandler;
        }

        public DefaultContextEventListener(T callerController, HandleDelegate aHandler) : base(callerController)
        {
            handler = aHandler;
        }

        public DefaultContextEventListener(T callerController, Int32? listenerCodeInteger) : base(callerController, listenerCodeInteger)
        {
        }

        private readonly HandleDelegate handler;

        public override void handle(Event anEvent)
        {
            handler(anEvent);
        }
    }
}