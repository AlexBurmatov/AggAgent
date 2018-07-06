using System;

namespace com.tibbo.aggregate.common.context
{
    public interface RequestController<T> where T : RequestData
    {
        object getOriginator();

        long? getLockTimeout();
    }
}