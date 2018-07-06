using System;
using com.tibbo.aggregate.common.data;

namespace com.tibbo.aggregate.common.context
{
    public interface ContextEventListener
    {
        Boolean shouldHandle(Event ev);

        void handle(Event anEvent);

        CallerController<CallerData> getCallerController();

        Int32? getListenerCode();
    }
}