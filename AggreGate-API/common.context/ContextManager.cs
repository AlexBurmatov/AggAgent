using System;
using com.tibbo.aggregate.common.data;

namespace com.tibbo.aggregate.common.context
{
    public interface ContextManager
    {
        void start();

        void restart();

        void stop();

        Context getRoot();

        Context get(String contextName, CallerController<CallerData> caller);

        Context get(String contextName);

        void addMaskEventListener(String mask, String eventString, ContextEventListener listener);

        void removeMaskEventListener(String mask, String eventString, ContextEventListener listener);

        void contextAdded(Context con);
        void contextRemoved(Context con);

        void contextInfoChanged(Context con);

        void variableAdded(Context con, VariableDefinition vd);
        void variableRemoved(Context con, VariableDefinition vd);

        void functionAdded(Context con, FunctionDefinition fd);
        void functionRemoved(Context con, FunctionDefinition fd);

        void eventAdded(Context con, EventDefinition ed);
        void eventRemoved(Context con, EventDefinition ed);

        void queue(EventData ed, Event ev);
    }
}